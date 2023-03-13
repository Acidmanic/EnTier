using System;

namespace Example.WebView.DoubleAggregate.HumanArea.Events
{
    public class TrustedHumanEvent:IHumanEvent
    {
        public DateTime Timestamp { get; set; }
        
        public string Weakness { get; set; }
        
        public string SuperPower { get; set; }
    }
}