using System;

namespace EnTier.Exceptions
{
    public class InvalidEventIdTypeException<TEventId> : InvalidEventIdTypeException
    {
        public InvalidEventIdTypeException() : base(typeof(TEventId))
        {
        }

        public InvalidEventIdTypeException(string message) : base(message)
        {
        }
    }

    public class InvalidEventIdTypeException : Exception
    {
        public InvalidEventIdTypeException(Type eventIdType) :
            base($"{eventIdType.FullName} is not a valid type for event id.")
        {
        }

        public InvalidEventIdTypeException(string message) : base(message)
        {
        }
    }
}