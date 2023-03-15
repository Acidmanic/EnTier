using System;

namespace Example.WebView.DoubleAggregate.HumanArea.Events
{
    public class IntroducedHumanEvent:IHumanEvent
    {
        public DateTime Timestamp { get; set; }
        
        public string Name { get; set; }
        
        public string Surname { get; set; }
        
        public string Job { get; set; }
        
        public Guid Id { get; set; }
    }
}