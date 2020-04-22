using Microsoft.Extensions.Configuration;
using System;

namespace OpenMts.EnvironmentReader.Provider
{
    /// <summary>
    /// A provider of mocked environmental humidity. Starts at a fixed value and increases/decreases by a random value on each read.
    /// </summary>
    /// <seealso cref="OpenMts.EnvironmentReader.IEnvironmentFactorProvider" />
    public class MockHumidityProvider : IEnvironmentFactorProvider
    {
        /// <summary>
        /// Random number generator.
        /// </summary>
        private Random Random { get; }

        /// <summary>
        /// Tracks the last humidity for a continuous data flow.
        /// </summary>
        private float LastHumidity { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="MockHumidityProvider"/> class.
        /// </summary>
        /// <param name="configuration">The configuration.</param>
        public MockHumidityProvider(IConfiguration configuration)
        {
            Random = new Random();
            LastHumidity = Random.Next(16, 24);
        }

        /// <summary>
        /// Reads the current value.
        /// </summary>
        public float Read()
        {
            float newValue;
            do
            {
                newValue = GenerateNewValue();
            } while (newValue < 5 || newValue > 80);
            LastHumidity = newValue;
            return LastHumidity;
        }

        /// <summary>
        /// Generates a new value.
        /// </summary>
        /// <returns>Returns a new value.</returns>
        private float GenerateNewValue()
        {
            double newValue = LastHumidity + Random.NextDouble() * (Random.Next(0, 2) == 0 ? -1 : 1);
            return (float)Math.Round(newValue, 1, MidpointRounding.AwayFromZero);
        }
    }
}
