namespace OpenMTS.Services.Exceptions
{
    /// <summary>
    /// Indicates that the given exception is of the "resource already exists" class of exceptions.
    /// </summary>
    public interface IResourceAlreadyExsistsException
    {
        /// <summary>
        /// Message detailing what resource already exists.
        /// </summary>
        string Message { get; }
    }
}
