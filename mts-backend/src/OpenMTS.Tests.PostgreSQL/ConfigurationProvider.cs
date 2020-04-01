using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace OpenMTS.Tests.PostgreSQL
{
    /// <summary>
    /// Provides configuration to test classes.
    /// </summary>
    public static class ConfigurationProvider
    {
        /// <summary>
        /// Gets the configuration.
        /// </summary>
        public static IConfiguration GetConfiguration()
        {
            return new ConfigurationBuilder().AddJsonFile("./testsettings.json").AddJsonFile("./testsettings.Development.json", true).Build();
        }
    }
}
