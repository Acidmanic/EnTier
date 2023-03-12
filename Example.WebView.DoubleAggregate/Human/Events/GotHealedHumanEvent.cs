using System;

namespace Example.WebView.DoubleAggregate.Human.Events
{
    public class GotHealedHumanEvent:IHumanEvent
    {
        public DateTime Timestamp { get; set; }
        
    }
}