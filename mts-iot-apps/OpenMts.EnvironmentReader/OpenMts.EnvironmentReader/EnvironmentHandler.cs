using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace OpenMts.EnvironmentReader
{
    /// <summary>
    /// Handles periodically reading from environmental data providers.
    /// </summary>
    public class EnvironmentHandler
    {
        /// <summary>
        /// Tracks providers to read data from.
        /// </summary>
        private Dictionary<Factor, IEnvironmentFactorProvider> Providers { get; }

        /// <summary>
        /// The Kafka topic to write data to.
        /// </summary>
        private string Topic { get; }

        /// <summary>
        /// The interval to read data in.
        /// </summary>
        private TimeSpan ReadInterval { get; }

        /// <summary>
        /// The task performing periodical reads.
        /// </summary>
        private Task ReadLoop { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="EnvironmentHandler"/> class.
        /// </summary>
        /// <param name="topic">The Kafka topic to write data to.</param>
        /// <param name="readInterval">The read interval.</param>
        public EnvironmentHandler(string topic, TimeSpan readInterval)
        {
            Providers = new Dictionary<Factor, IEnvironmentFactorProvider>();
            Topic = topic;
            ReadInterval = readInterval;
            ReadLoop = null;
        }

        /// <summary>
        /// Registers a new data provider.
        /// </summary>
        /// <param name="factor">The factor that provider provides data for.</param>
        /// <param name="provider">The provider.</param>
        public void RegisterProvider(Factor factor, IEnvironmentFactorProvider provider)
        {
            Providers.Add(factor, provider);
        }

        /// <summary>
        /// Gets all factors being tracked (i.e. that there are providers registered for).
        /// </summary>
        /// <returns>Returns a list of factors.</returns>
        public IEnumerable<Factor> GetTrackedFactors()
        {
            return Providers.Select(p => p.Key);
        }

        /// <summary>
        /// Starts periodically reading data.
        /// <paramref name="cancellationTokenSoruce"/>
        /// </summary>
        public void StartReading(CancellationTokenSource cancellationTokenSoruce)
        {
            if (ReadLoop == null)
            {
                ReadLoop = new TaskFactory().StartNew(async () =>
                {
                    while (!cancellationTokenSoruce.IsCancellationRequested)
                    {
                        DataPoint dataPoint = new DataPoint()
                        {
                            Timestamp = DateTime.UtcNow,
                            Temperature = null,
                            Humidity = null
                        };
                        IEnvironmentFactorProvider provider = null;
                        if (Providers.TryGetValue(Factor.Temperature, out provider))
                        {
                            dataPoint.Temperature = provider.Read();
                        }
                        if (Providers.TryGetValue(Factor.Humidity, out provider))
                        {
                            dataPoint.Humidity = provider.Read();
                        }
                        // TODO: Write to Kafka
                        Console.WriteLine($" + {dataPoint.Timestamp} - Temperature: {dataPoint.Temperature} °C - Humidity: {dataPoint.Humidity} g/m^3");
                        await Task.Delay(ReadInterval);
                    }
                    Console.WriteLine("Environment handler terminating...");
                }, TaskCreationOptions.LongRunning);
            }
        }
    }
}
