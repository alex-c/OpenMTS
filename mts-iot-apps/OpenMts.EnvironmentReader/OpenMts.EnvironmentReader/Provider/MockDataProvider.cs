using Microsoft.Extensions.Configuration;
using System;

namespace OpenMts.EnvironmentReader.Provider
{
    /// <summary>
    /// A provider of mocked environmental data. Starts at a fixed value and increases/decreases by a random value on each read.
    /// </summary>
    /// <seealso cref="OpenMts.EnvironmentReader.IEnvironmentFactorProvider" />
    public class MockDataProvider : IEnvironmentFactorProvider
    {
        /// <summary>
        /// Random number generator.
        /// </summary>
        private Random Random { get; }

        /// <summary>
        /// Tracks the last temperature for a continuous data flow.
        /// </summary>
        private double LastTemperature { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="MockDataProvider"/> class.
        /// </summary>
        /// <param name="configuration">The configuration.</param>
        public MockDataProvider(IConfiguration configuration)
        {
            Random = new Random();
            LastTemperature = Random.Next(16, 24);
        }

        /// <summary>
        /// Reads the current value.
        /// </summary>
        public double Read()
        {
            LastTemperature += Random.NextDouble() * (Random.Next(0, 2) == 0 ? -1 : 1);
            return LastTemperature;
        }
    }
}
