using System;

namespace EnTier.Query.Attributes
{
    public class FilterResultExpirationDurationAttribute:Attribute
    {
        public FilterResultExpirationDurationAttribute(long milliseconds)
        {
            Milliseconds = milliseconds;
        }

        public long Milliseconds { get; }
    }
}