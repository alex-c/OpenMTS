using Microsoft.Extensions.Configuration;
using OpenMts.EnvironmentReader.Provider;
using System;
using System.Linq;
using System.Threading;

namespace OpenMts.EnvironmentReader
{
    /// <summary>
    /// An IoT app to read environmental data (like temperature) for a specific OpenMTS storage site.
    /// </summary>
    public class Program
    {
        /// <summary>
        /// Program entry points. Parses config, initializes providers and starts a handler.
        /// </summary>
        static void Main(string[] args)
        {
            ConsoleKeyInfo ConsoleKeyInfo;
            Console.TreatControlCAsInput = true;

            // Read config files
            IConfiguration configuration = new ConfigurationBuilder()
                .AddJsonFile("./appsettings.json")
                .AddJsonFile("./appsettings.Development.json", true).Build();

            // Get and validate topic
            string topic = configuration.GetValue("Topic", "");
            if (string.IsNullOrWhiteSpace(topic))
            {
                throw new Exception("No valid Kafka topic name was provided.");
            }
            else
            {
                Console.WriteLine($"Configuring Kafka topic `{topic}`...");
            }

            // Get and validate read interval
            int readIntervalInSeconds = configuration.GetValue("ReadIntervalInSeconds", 10);
            if (readIntervalInSeconds <= 0)
            {
                throw new Exception("No valid read interval was provided.");
            }
            else
            {
                Console.WriteLine($"Configuring a read interval of `{readIntervalInSeconds}` seconds...");
            }
            TimeSpan readInterval = TimeSpan.FromSeconds(readIntervalInSeconds);

            // Create handler
            EnvironmentHandler handler = new EnvironmentHandler(topic, readInterval);

            // Temperature provider
            IEnvironmentFactorProvider temperatureProvider = CreateProvider(configuration, Factor.Temperature);
            if (temperatureProvider != null)
            {
                handler.RegisterProvider(Factor.Temperature, temperatureProvider);
            }

            // Humidity provider
            IEnvironmentFactorProvider humidityProvider = CreateProvider(configuration, Factor.Humidity);
            if (humidityProvider != null)
            {
                handler.RegisterProvider(Factor.Humidity, humidityProvider);
            }

            // Check for any provider...
            if (!handler.GetTrackedFactors().Any())
            {
                throw new Exception("No providers loaded, terminating...");
            }


            // Start reading data!
            CancellationTokenSource cancellationTokenSoruce = new CancellationTokenSource();
            handler.StartReading(cancellationTokenSoruce);

            // Wait for CTRL+C termination
            do
            {
                Console.WriteLine("Please enter CTRL+C to terminate...");
                ConsoleKeyInfo = Console.ReadKey(true);
            } while ((ConsoleKeyInfo.Modifiers & ConsoleModifiers.Control) == 0 || ConsoleKeyInfo.Key != ConsoleKey.C);
            cancellationTokenSoruce.Cancel();
            Console.WriteLine("Cancellation was requested, waiting for environment handler to terminate...");
            Thread.Sleep(readInterval*2);
        }

        /// <summary>
        /// Creates a environmental data provider from configuration.
        /// </summary>
        /// <param name="configuration">The configuration to get provider info from.</param>
        /// <param name="factor">The factor to create a provider for.</param>
        /// <returns>Returns a provider or null.</returns>
        /// <exception cref="System.Exception">An invalid provider name `{providerName}` was supplied for factor `{factor.ToString().ToLower()}`.</exception>
        private static IEnvironmentFactorProvider CreateProvider(IConfiguration configuration, Factor factor)
        {
            string providerName = configuration.GetValue($"{factor.ToString()}:Provider", "");
            if (string.IsNullOrWhiteSpace(providerName))
            {
                Console.WriteLine($"No provider configured for factor `{factor.ToString().ToLower()}`, ignoring...");
                return null;
            }
            else
            {
                IEnvironmentFactorProvider provider = null;
                switch (providerName)
                {
                    case "MockDataProvider":
                        provider = new MockDataProvider(configuration.GetSection($"{factor.ToString()}:Configuration"));
                        break;
                    default:
                        throw new Exception($"An invalid provider name `{providerName}` was supplied for factor `{factor.ToString().ToLower()}`.");
                }
                Console.WriteLine($"Configuring provider `{providerName}` for factor `{factor.ToString().ToLower()}`.");
                return provider;
            }
        }
    }
}
