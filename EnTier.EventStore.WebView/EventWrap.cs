namespace EnTier.EventStore.WebView
{
    public class EventWrap
    {
        public object EventId { get; set; }
        
        public object StreamId { get; set; }
        
        public object Event { get; set; }
    }
}