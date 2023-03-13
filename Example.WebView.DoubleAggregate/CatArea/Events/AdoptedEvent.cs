namespace Example.WebView.DoubleAggregate.CatArea.Events
{
    public class AdoptedEvent :ICatEvent
    {
        
        
        public long StreamId { get; set; } 
        public long Timestamp { get; set; }
        
        public string Name { get; set; }
        
        public double Weight { get; set; }
        
        
    }
}