using System;

namespace EnTier.EventSourcing.Attributes
{
    public class EventStoreIdentifierAttribute:Attribute
    {
        public EventStoreIdentifierAttribute(Type eventIdType)
        {
            EventIdType = eventIdType;
        }

        public Type EventIdType { get; }
    }
}