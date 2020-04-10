namespace OpenMTS.Services.Support
{
    /// <summary>
    /// Publisher interface of a minimal pub/sub pattern.
    /// </summary>
    /// <typeparam name="T">Entity type to publish.</typeparam>
    public interface IPublisher<T>
    {
        /// <summary>
        /// Subscribes the specified subscriber.
        /// </summary>
        /// <param name="subscriber">The subscriber.</param>
        void Subscribe(ISubscriber<T> subscriber);

        /// <summary>
        /// Unsubscribes the specified subscriber.
        /// </summary>
        /// <param name="subscriber">The subscriber.</param>
        void Unsubscribe(ISubscriber<T> subscriber);

        /// <summary>
        /// Publishes an entity.
        /// </summary>
        /// <param name="entity">The entity to publish.</param>
        void Publish(T entity);
    }
}
