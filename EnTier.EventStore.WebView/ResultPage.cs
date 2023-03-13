using System.Collections.Generic;

namespace EnTier.EventStore.WebView
{
    public class ResultPage
    {

        public int TotalCount { get; set; } = 0;

        public List<EventWrap> Events { get; set; } = new List<EventWrap>();


    }
}