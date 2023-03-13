using System;

namespace Example.WebView.DoubleAggregate.HumanArea.Events
{
    public class GotIlleHumanEvent:IHumanEvent
    {
        public DateTime Timestamp { get; set; }
        
        public string Sickness { get; set; }
        
    }
}