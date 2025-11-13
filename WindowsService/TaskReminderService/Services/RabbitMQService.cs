using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

namespace TaskReminderService.Services
{
    public class RabbitMQService : IRabbitMQService, IDisposable
    {
        private readonly ILogger<RabbitMQService> _logger;
        private readonly IConnection _connection;
        private readonly IModel _channel;
        private readonly string _queueName;
        private readonly string _deadLetterQueueName;

        public RabbitMQService(IConfiguration configuration, ILogger<RabbitMQService> logger)
        {
            _logger = logger;

            var factory = new ConnectionFactory
            {
                HostName = configuration["RabbitMQ:HostName"],
                Port = int.Parse(configuration["RabbitMQ:Port"] ?? "5672"),
                UserName = configuration["RabbitMQ:UserName"],
                Password = configuration["RabbitMQ:Password"],
                // Enable automatic connection recovery
                AutomaticRecoveryEnabled = true,
                NetworkRecoveryInterval = TimeSpan.FromSeconds(10)
            };

            _queueName = configuration["RabbitMQ:QueueName"] ?? "task-reminders";
            _deadLetterQueueName = $"{_queueName}-dlq";

            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();

            // Declare dead letter queue
            _channel.QueueDeclare(
                queue: _deadLetterQueueName,
                durable: true,
                exclusive: false,
                autoDelete: false,
                arguments: null);

            // Declare main queue with dead letter exchange
            var queueArgs = new Dictionary<string, object>
            {
                { "x-dead-letter-exchange", "" },
                { "x-dead-letter-routing-key", _deadLetterQueueName }
            };

            _channel.QueueDeclare(
                queue: _queueName,
                durable: true,
                exclusive: false,
                autoDelete: false,
                arguments: queueArgs);

            // Set prefetch count to limit concurrent message processing
            _channel.BasicQos(prefetchSize: 0, prefetchCount: 10, global: false);

            _logger.LogInformation("RabbitMQ connection established. Queue: {QueueName}, DLQ: {DLQName}", 
                _queueName, _deadLetterQueueName);
        }

        public void PublishMessage(string message)
        {
            var body = Encoding.UTF8.GetBytes(message);
            var properties = _channel.CreateBasicProperties();
            properties.Persistent = true;
            properties.DeliveryMode = 2; // Persistent
            properties.MessageId = Guid.NewGuid().ToString();
            properties.Timestamp = new AmqpTimestamp(DateTimeOffset.UtcNow.ToUnixTimeSeconds());

            _channel.BasicPublish(
                exchange: "",
                routingKey: _queueName,
                basicProperties: properties,
                body: body);

            _logger.LogInformation("Message published to queue with ID: {MessageId}", properties.MessageId);
        }

        public void StartConsuming(Action<string> onMessageReceived)
        {
            var consumer = new EventingBasicConsumer(_channel);
            consumer.Received += (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                var messageId = ea.BasicProperties.MessageId;

                try
                {
                    _logger.LogInformation("Processing message ID: {MessageId}", messageId);
                    onMessageReceived(message);
                    
                    // Acknowledge successful processing
                    _channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);
                    _logger.LogInformation("Message acknowledged: {MessageId}", messageId);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error processing message ID: {MessageId}", messageId);
                    
                    // Check retry count from headers
                    var retryCount = 0;
                    if (ea.BasicProperties.Headers != null && 
                        ea.BasicProperties.Headers.ContainsKey("x-retry-count"))
                    {
                        retryCount = Convert.ToInt32(ea.BasicProperties.Headers["x-retry-count"]);
                    }

                    if (retryCount < 3)
                    {
                        // Retry: send back to queue with incremented retry count
                        var retryProperties = _channel.CreateBasicProperties();
                        retryProperties.Persistent = true;
                        retryProperties.MessageId = messageId;
                        retryProperties.Headers = new Dictionary<string, object>
                        {
                            { "x-retry-count", retryCount + 1 },
                            { "x-error", ex.Message }
                        };

                        _channel.BasicPublish(
                            exchange: "",
                            routingKey: _queueName,
                            basicProperties: retryProperties,
                            body: body);

                        _channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);
                        _logger.LogWarning("Message requeued for retry {RetryCount}/3: {MessageId}", 
                            retryCount + 1, messageId);
                    }
                    else
                    {
                        // Max retries exceeded, reject to dead letter queue
                        _channel.BasicReject(deliveryTag: ea.DeliveryTag, requeue: false);
                        _logger.LogError("Message moved to DLQ after 3 retries: {MessageId}", messageId);
                    }
                }
            };

            _channel.BasicConsume(
                queue: _queueName,
                autoAck: false,
                consumer: consumer);

            _logger.LogInformation("Started consuming messages from queue: {QueueName}", _queueName);
        }

        public void Dispose()
        {
            _channel?.Close();
            _connection?.Close();
            _logger.LogInformation("RabbitMQ connection closed");
        }
    }
}
