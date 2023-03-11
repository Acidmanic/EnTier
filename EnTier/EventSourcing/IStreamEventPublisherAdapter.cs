namespace EnTier.EventSourcing
{

    /// <summary>
    /// This interface is used to introduce an event publisher to EnTier so stream-repositories can use
    /// it to publish their events after being stored..
    /// </summary>
    public interface IStreamEventPublisherAdapter
    {
        void Publish(object @event, object eventId, object streamId);
    }
}