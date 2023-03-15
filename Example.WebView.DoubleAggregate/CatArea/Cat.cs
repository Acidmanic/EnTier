using System;
using System.Collections.Generic;

namespace Example.WebView.DoubleAggregate.CatArea
{
    public class Cat
    {
        public long Id { get; set; }
        
        public string Name { get; set; }
        
        public bool Sterilized { get; set; }
        
        public double Weight { get; set; }
        
        
        public bool LivesOnStreet { get; set; }
        
        public List<string> AllNames { get; set; }
        
        public DateTime LastUpdate { get; set; }
    }
}