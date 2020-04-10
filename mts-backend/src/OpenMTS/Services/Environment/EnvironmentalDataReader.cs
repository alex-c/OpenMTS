using Confluent.Kafka;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using OpenMTS.Models;
using OpenMTS.Models.Environmnt;
using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

namespace OpenMTS.Services.Environment
{
    /// <summary>
    /// Reads data from Kafka for a given storage site.
    /// </summary>
    public class EnvironmentalDataReader
    {
        /// <summary>
        /// The Kafka consumer configuration.
        /// </summary>
        private ConsumerConfig ConsumerConfig { get; }

        /// <summary>
        /// Thread-safe queue to write data to.
        /// </summary>
        private ConcurrentQueue<EnvironmentSnapshot> SnapshotQueue { get; }

        /// <summary>
        /// The ID of the storage site to read data for.
        /// </summary>
        private Guid StorageSiteId { get; }

        /// <summary>
        /// Kafka topic to read data from.
        /// </summary>
        private string Topic { get; }

        /// <summary>
        /// Used to log info.
        /// </summary>
        private ILogger Logger { get; }

        /// <summary>
        /// The worker task actually reading data from Kafka.
        /// </summary>
        private Task Worker { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="EnvironmentalDataReader"/> class.
        /// </summary>
        /// <param name="logger">Used to log information.</param>
        /// <param name="snapshotQueue">Thread-safe queue to write data to.</param>
        /// <param name="kafkaEndpoint">The Kafka endpoint to read data from.</param>
        /// <param name="site">The storage site for which to read data.</param>
        public EnvironmentalDataReader(ILogger<EnvironmentalDataReader> logger,
            ConcurrentQueue<EnvironmentSnapshot> snapshotQueue,
            string kafkaEndpoint,
            StorageSite site)
        {
            StorageSiteId = site.Id;
            SnapshotQueue = snapshotQueue;
            ConsumerConfig = new ConsumerConfig
            {
                GroupId = "openmts-environment-consumers",
                BootstrapServers = kafkaEndpoint,
                AutoOffsetReset = AutoOffsetReset.Earliest
            };
            Topic = $"openmts-environment-{StorageSiteId}";
            Logger = logger;

            // Start consuming
            Worker = StartConsuming();
        }

        /// <summary>
        /// Starts and returns a task that reads environmental data from Kafka.
        /// </summary>
        /// <returns>Returns the new task.</returns>
        private Task StartConsuming()
        {
            return new TaskFactory().StartNew(() =>
            {
                using (IConsumer<Ignore, string> consumer = new ConsumerBuilder<Ignore, string>(ConsumerConfig).Build())
                {
                    Logger.LogInformation($" + Starting to read environment data from topic `{Topic}`.");
                    consumer.Subscribe(Topic);

                    CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
                    Console.CancelKeyPress += (_, @event) => {
                        @event.Cancel = true;
                        cancellationTokenSource.Cancel();
                    };

                    try
                    {
                        while (true)
                        {
                            try
                            {
                                ConsumeResult<Ignore, string> result = consumer.Consume(cancellationTokenSource.Token);
                                EnvironmentSnapshot snapshot = JsonConvert.DeserializeObject<EnvironmentSnapshot>(result.Message.Value);
                                snapshot.StorageSiteId = StorageSiteId;
                                SnapshotQueue.Enqueue(snapshot);
                                Logger.LogDebug($"Consumed message '{result.Message.Value}' at: '{result.TopicPartitionOffset}'.");
                            }
                            catch (ConsumeException exception)
                            {
                                Logger.LogError($"Error occured: {exception.Error.Reason}");
                            }
                        }
                    }
                    catch (OperationCanceledException)
                    {
                        consumer.Close();
                    }
                    finally
                    {
                        Logger.LogInformation($"Kafka consumer for topic `{Topic}` shut down.");
                    }
                }
            }, TaskCreationOptions.LongRunning);
        }
    }
}
