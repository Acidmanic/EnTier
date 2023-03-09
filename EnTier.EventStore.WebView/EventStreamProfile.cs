using System;

namespace EnTier.EventStore.WebView
{
    internal class EventStreamProfile
    {
        public Type AggregateType { get; set; }
        
        public Type EventType { get; set; }
        
        public Type AggregateRootType { get; set; }
        
        public Type StreamIdType { get; set; }
        
        
        public Type EventIdType { get; set; }
    }
}