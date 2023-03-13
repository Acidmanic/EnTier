namespace Example.WebView.DoubleAggregate.CatArea.Events
{
    public class FattenedEvent:ICatEvent
    {
        public long Timestamp { get; set; }
        
        public double HowMuch { get; set; }
        
    }
}