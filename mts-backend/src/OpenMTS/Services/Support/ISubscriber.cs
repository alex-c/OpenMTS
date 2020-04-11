namespace OpenMTS.Services.Support
{
    /// <summary>
    /// Subscriber interface of a minimal pub/sub pattern.
    /// </summary>
    /// <typeparam name="T">Entity type to subscribe to.</typeparam>
    public interface ISubscriber<T>
    {
        /// <summary>
        /// Called when an <see cref="IPublisher{T}"/> publishes an entity.
        /// </summary>
        /// <param name="entity">The published entity.</param>
        void OnPublish(T entity);
    }
}
