using OpenMTS.Models;

namespace OpenMTS.Repositories
{
    /// <summary>
    /// A repository that allows to get/set the OpenMTS configuration.
    /// </summary>
    public interface IConfigurationRepository
    {
        /// <summary>
        /// Gets the current OpenMTS configuration.
        /// </summary>
        /// <returns>Returns the current configuration.</returns>
        Configuration GetConfiguration();

        /// <summary>
        /// Sets the current OpenMTS configuration.
        /// </summary>
        /// <param name="configuration">The configuration to set.</param>
        void SetConfiguration(Configuration configuration);
    }
}
