using Confluent.Kafka;
using System.Text.Json;

namespace FraudDetectionAPI.Services
{
    /// <summary>
    /// Kafka event streaming service for real-time fraud detection events
    /// </summary>
    public interface IKafkaService
    {
        Task PublishTransactionAsync(object transaction);
        Task PublishFraudAlertAsync(object alert);
        Task PublishEventAsync(string topic, object message);
    }

    public class KafkaService : IKafkaService, IDisposable
    {
        private readonly IProducer<string, string>? _producer;
        private readonly ILogger<KafkaService> _logger;
        private readonly bool _isEnabled;
        private readonly string _bootstrapServers;

        // Kafka Topics
        public const string TransactionsTopic = "fraudguard-transactions";
        public const string FraudAlertsTopic = "fraudguard-fraud-alerts";
        public const string AuditLogTopic = "fraudguard-audit-log";

        public KafkaService(IConfiguration configuration, ILogger<KafkaService> logger)
        {
            _logger = logger;
            _bootstrapServers = configuration["Kafka:BootstrapServers"] ?? "localhost:29092";
            _isEnabled = configuration.GetValue<bool>("Kafka:Enabled", false);

            if (_isEnabled)
            {
                try
                {
                    var config = new ProducerConfig
                    {
                        BootstrapServers = _bootstrapServers,
                        Acks = Acks.All,
                        EnableIdempotence = true,
                        MessageSendMaxRetries = 3,
                        RetryBackoffMs = 1000
                    };

                    _producer = new ProducerBuilder<string, string>(config).Build();
                    _logger.LogInformation("✅ Kafka producer connected to {Servers}", _bootstrapServers);
                }
                catch (Exception ex)
                {
                    _logger.LogWarning("⚠️ Kafka initialization failed: {Message}", ex.Message);
                    _isEnabled = false;
                }
            }
            else
            {
                _logger.LogInformation("ℹ️ Kafka disabled - running in standalone mode");
            }
        }

        public async Task PublishTransactionAsync(object transaction)
        {
            await PublishEventAsync(TransactionsTopic, transaction);
        }

        public async Task PublishFraudAlertAsync(object alert)
        {
            await PublishEventAsync(FraudAlertsTopic, alert);
        }

        public async Task PublishEventAsync(string topic, object message)
        {
            if (!_isEnabled || _producer == null)
            {
                _logger.LogDebug("Kafka disabled - skipping event publish to {Topic}", topic);
                return;
            }

            try
            {
                var jsonMessage = JsonSerializer.Serialize(message);
                var kafkaMessage = new Message<string, string>
                {
                    Key = Guid.NewGuid().ToString(),
                    Value = jsonMessage,
                    Timestamp = new Timestamp(DateTime.UtcNow)
                };

                var deliveryResult = await _producer.ProduceAsync(topic, kafkaMessage);
                
                _logger.LogDebug("Published event to {Topic} at partition {Partition}, offset {Offset}",
                    topic, deliveryResult.Partition.Value, deliveryResult.Offset.Value);
            }
            catch (ProduceException<string, string> ex)
            {
                _logger.LogError("Failed to publish event to {Topic}: {Error}", topic, ex.Error.Reason);
            }
        }

        public void Dispose()
        {
            _producer?.Dispose();
        }
    }
}
