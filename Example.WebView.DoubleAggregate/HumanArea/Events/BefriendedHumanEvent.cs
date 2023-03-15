using System;

namespace Example.WebView.DoubleAggregate.HumanArea.Events
{
    public class BefriendedHumanEvent:IHumanEvent
    {
        public DateTime Timestamp { get; set; }
        
        public string FavoriteFood { get; set; }
        
        public string FavoriteColor { get; set; }
    }
}