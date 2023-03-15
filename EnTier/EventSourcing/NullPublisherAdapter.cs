namespace EnTier.EventSourcing
{

    internal class NullPublisherAdapter : IStreamEventPublisherAdapter
    {
        public void Publish(object @event, object streamId, object eventId)
        {
            // Meeh!
        }
    }
}