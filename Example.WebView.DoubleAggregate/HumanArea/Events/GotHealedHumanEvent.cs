using System;

namespace Example.WebView.DoubleAggregate.HumanArea.Events
{
    public class GotHealedHumanEvent:IHumanEvent
    {
        public DateTime Timestamp { get; set; }
        
    }
}