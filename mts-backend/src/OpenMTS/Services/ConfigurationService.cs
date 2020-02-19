using OpenMTS.Models;
using OpenMTS.Repositories;

namespace OpenMTS.Services
{
    /// <summary>
    /// Provides functionality to get and set OpenMTS configuration.
    /// </summary>
    public class ConfigurationService
    {
        /// <summary>
        /// Provides access to the configuration.
        /// </summary>
        private IConfigurationRepository ConfigurationRepository { get; }

        /// <summary>
        /// Sets up the configuration service.
        /// </summary>
        /// <param name="configurationRepository">A repository for access to configuration.</param>
        public ConfigurationService(IConfigurationRepository configurationRepository)
        {
            ConfigurationRepository = configurationRepository;
        }

        /// <summary>
        /// Gets the current OpenMTS configuration.
        /// </summary>
        /// <returns>Returns the current configuration.</returns>
        public Configuration GetConfiguration()
        {
            return ConfigurationRepository.GetConfiguration();
        }

        /// <summary>
        /// Sets the OpenMTS configuration.
        /// </summary>
        /// <param name="configuration">The configuration to set.</param>
        public void SetConfiguration(Configuration configuration)
        {
            ConfigurationRepository.SetConfiguration(configuration);
        }

    }
}
