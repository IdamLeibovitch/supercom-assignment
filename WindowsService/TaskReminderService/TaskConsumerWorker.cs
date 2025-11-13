using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;
using System.Text.Json;
using TaskReminderService.Services;
using Backend.Data;

namespace TaskReminderService
{
    public class TaskConsumerWorker : BackgroundService
    {
        private readonly ILogger<TaskConsumerWorker> _logger;
        private readonly IRabbitMQService _rabbitMQService;
        private readonly IServiceProvider _serviceProvider;
        
        // Track processed messages to prevent duplicate logging (idempotency)
        private readonly ConcurrentDictionary<int, DateTime> _processedTasks = new();

        public TaskConsumerWorker(
            ILogger<TaskConsumerWorker> logger,
            IRabbitMQService rabbitMQService,
            IServiceProvider serviceProvider)
        {
            _logger = logger;
            _rabbitMQService = rabbitMQService;
            _serviceProvider = serviceProvider;
            
            // Clean up old entries every hour
            _ = Task.Run(async () => await CleanupProcessedTasksAsync());
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Task Consumer Worker started at: {time}", DateTimeOffset.Now);

            _rabbitMQService.StartConsuming(async message =>
            {
                await ProcessMessageAsync(message, stoppingToken);
            });

            return Task.CompletedTask;
        }

        private async Task ProcessMessageAsync(string message, CancellationToken stoppingToken)
        {
            try
            {
                var taskInfo = JsonSerializer.Deserialize<JsonElement>(message);
                var taskId = taskInfo.GetProperty("Id").GetInt32();
                var title = taskInfo.GetProperty("Title").GetString();

                // Idempotency check: has this task already been processed recently?
                if (_processedTasks.TryGetValue(taskId, out var processedTime))
                {
                    if (DateTime.UtcNow - processedTime < TimeSpan.FromHours(1))
                    {
                        _logger.LogInformation(
                            "Task {TaskId} already processed at {ProcessedTime}, skipping duplicate", 
                            taskId, processedTime);
                        return;
                    }
                }

                // Verify task still needs reminder by checking database
                using var scope = _serviceProvider.CreateScope();
                var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                
                var task = await dbContext.Tasks.FindAsync(new object[] { taskId }, stoppingToken);
                
                if (task == null)
                {
                    _logger.LogWarning("Task {TaskId} not found in database", taskId);
                    return;
                }

                if (!task.IsReminderSent)
                {
                    _logger.LogWarning(
                        "Task {TaskId} received from queue but not marked as sent in database", 
                        taskId);
                }

                // Log the reminder message
                var formattedMessage = $"Hi your Task is due {{Task {title}}}";
                _logger.LogInformation(formattedMessage);

                // Mark as processed (idempotency)
                _processedTasks.TryAdd(taskId, DateTime.UtcNow);

                // Here you could add additional logic:
                // - Send email notification
                // - Send SMS
                // - Trigger webhooks
                // - Update external systems
            }
            catch (JsonException ex)
            {
                _logger.LogError(ex, "Invalid JSON format in message: {Message}", message);
                throw; // Let RabbitMQ service handle retry/DLQ
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing message: {Message}", message);
                throw; // Let RabbitMQ service handle retry/DLQ
            }
        }

        private async Task CleanupProcessedTasksAsync()
        {
            while (true)
            {
                await Task.Delay(TimeSpan.FromHours(1));
                
                var cutoffTime = DateTime.UtcNow.AddHours(-2);
                var keysToRemove = _processedTasks
                    .Where(kvp => kvp.Value < cutoffTime)
                    .Select(kvp => kvp.Key)
                    .ToList();

                foreach (var key in keysToRemove)
                {
                    _processedTasks.TryRemove(key, out _);
                }

                _logger.LogInformation("Cleaned up {Count} old processed task entries", keysToRemove.Count);
            }
        }
    }
}
