namespace Example.WebView.DoubleAggregate.CatArea.Events
{
    public class ReNamedEvent:ICatEvent
    {
        public long Timestamp { get; set; }
        
        public string Name { get; set; }
    }
}