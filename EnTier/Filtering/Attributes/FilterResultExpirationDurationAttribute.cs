using System;

namespace EnTier.Filtering.Attributes
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