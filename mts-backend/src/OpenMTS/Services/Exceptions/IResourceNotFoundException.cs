namespace OpenMTS.Services.Exceptions
{
    /// <summary>
    /// Indicates that the given exception is of the "resource not found" class of exceptions.
    /// </summary>
    public interface IResourceNotFoundException
    {
        /// <summary>
        /// Message detailing what resource could not be found.
        /// </summary>
        string Message { get; }
    }
}
