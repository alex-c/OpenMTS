namespace OpenMts.EnvironmentReader
{
    /// <summary>
    /// A generic interface for a provider of values for a specific environmental factor.
    /// </summary>
    public interface IEnvironmentFactorProvider
    {
        /// <summary>
        /// Reads the current value.
        /// </summary>
        /// <returns>Returns the current value.</returns>
        double Read();
    }
}
