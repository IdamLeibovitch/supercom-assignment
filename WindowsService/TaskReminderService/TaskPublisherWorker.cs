using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using TaskReminderService.Services;
using Backend.Data;

namespace TaskReminderService
{
    public class TaskPublisherWorker : BackgroundService
    {
        private readonly ILogger<TaskPublisherWorker> _logger;
        private readonly IServiceProvider _serviceProvider;
        private readonly IRabbitMQService _rabbitMQService;
        private readonly int _checkIntervalMinutes;

        public TaskPublisherWorker(
            ILogger<TaskPublisherWorker> logger,
            IServiceProvider serviceProvider,
            IRabbitMQService rabbitMQService,
            IConfiguration configuration)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
            _rabbitMQService = rabbitMQService;
            _checkIntervalMinutes = int.Parse(configuration["TaskReminder:CheckIntervalMinutes"] ?? "5");
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Task Publisher Worker started at: {time}", DateTimeOffset.Now);

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await ProcessOverdueTasksAsync(stoppingToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error occurred while checking for overdue tasks");
                }

                await Task.Delay(TimeSpan.FromMinutes(_checkIntervalMinutes), stoppingToken);
            }
        }

        private async Task ProcessOverdueTasksAsync(CancellationToken stoppingToken)
        {
            using var scope = _serviceProvider.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            await using var transaction = await dbContext.Database.BeginTransactionAsync(stoppingToken);

            try
            {
                var overdueTasks = await dbContext.Tasks
                    .Where(t => t.DueDate < DateTime.UtcNow && !t.IsReminderSent)
                    .ToListAsync(stoppingToken);

                foreach (var task in overdueTasks)
                {
                    var success = await ProcessTaskWithRetryAsync(dbContext, task, stoppingToken);
                    
                    if (!success)
                    {
                        _logger.LogWarning("Failed to process task {TaskId} after retries", task.Id);
                    }
                }

                await transaction.CommitAsync(stoppingToken);
                _logger.LogInformation("Processed {Count} overdue tasks", overdueTasks.Count);
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync(stoppingToken);
                _logger.LogError(ex, "Transaction rolled back due to error");
                throw;
            }
        }

        private async Task<bool> ProcessTaskWithRetryAsync(
            ApplicationDbContext dbContext, 
            Backend.Data.Models.Task task, 
            CancellationToken stoppingToken)
        {
            const int maxRetries = 3; // for mechanism sake
            var retryCount = 0;

            while (retryCount < maxRetries)
            {
                try
                {
                    await dbContext.Entry(task).ReloadAsync(stoppingToken);

                    if (task.IsReminderSent)
                    {
                        _logger.LogInformation(
                            "Task {TaskId} already processed by another instance", task.Id);
                        return true;
                    }

                    var taskJson = JsonSerializer.Serialize(new
                    {
                        task.Id,
                        task.Title
                    });

                    // Publish to queue first
                    _rabbitMQService.PublishMessage(taskJson);

                    // Update database
                    task.IsReminderSent = true;
                    task.ReminderSentAt = DateTime.UtcNow;
                    task.ReminderRetryCount = retryCount;

                    await dbContext.SaveChangesAsync(stoppingToken);

                    _logger.LogInformation(
                        "Successfully processed task {TaskId} on attempt {Attempt}", 
                        task.Id, retryCount + 1);
                    return true;
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    retryCount++;
                    _logger.LogWarning(ex, 
                        "Concurrency conflict for task {TaskId}, retry {Retry}/{MaxRetries}", 
                        task.Id, retryCount, maxRetries);

                    if (retryCount >= maxRetries)
                    {
                        task.LastProcessingError = $"Concurrency error after {maxRetries} retries";
                        await dbContext.SaveChangesAsync(stoppingToken);
                        return false;
                    }

                    // Exponential backoff
                    await Task.Delay(TimeSpan.FromMilliseconds(100 * Math.Pow(2, retryCount)), stoppingToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error processing task {TaskId}", task.Id);
                    task.LastProcessingError = ex.Message;
                    await dbContext.SaveChangesAsync(stoppingToken);
                    return false;
                }
            }

            return false;
        }
    }
}
