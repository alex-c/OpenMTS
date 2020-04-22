using Microsoft.Extensions.Configuration;
using System;

namespace OpenMts.EnvironmentReader.Provider
{
    /// <summary>
    /// A provider of mocked environmental temperature. Starts at a fixed value and increases/decreases by a random value on each read.
    /// </summary>
    /// <seealso cref="OpenMts.EnvironmentReader.IEnvironmentFactorProvider" />
    public class MockTemperatureProvider : IEnvironmentFactorProvider
    {
        /// <summary>
        /// Random number generator.
        /// </summary>
        private Random Random { get; }

        /// <summary>
        /// Tracks the last temperature for a continuous data flow.
        /// </summary>
        private float LastTemperature { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="MockTemperatureProvider"/> class.
        /// </summary>
        /// <param name="configuration">The configuration.</param>
        public MockTemperatureProvider(IConfiguration configuration)
        {
            Random = new Random();
            LastTemperature = Random.Next(16, 24);
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
            } while (newValue < -10 || newValue > 66);
            LastTemperature = newValue;
            return LastTemperature;
        }

        /// <summary>
        /// Generates a new value.
        /// </summary>
        /// <returns>Returns a new value.</returns>
        private float GenerateNewValue()
        {
            double newValue = LastTemperature + Random.NextDouble() * (Random.Next(0, 2) == 0 ? -1 : 1);
            return (float)Math.Round(newValue, 2, MidpointRounding.AwayFromZero);
        }
    }
}
