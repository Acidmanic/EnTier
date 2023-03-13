namespace Example.WebView.DoubleAggregate.CatArea.Events
{
    public class LostWeight:ICatEvent
    {
        public long Timestamp { get; set; }
        
        public double HowMuch { get; set; }
        
    }
}