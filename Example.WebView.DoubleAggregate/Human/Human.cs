using System;
using System.Collections.Generic;

namespace Example.WebView.DoubleAggregate.Human
{
    public class Human
    {
        public Guid Id { get; set; }
        
        public string Name { get; set; }
        
        public string Surname { get; set; }
        
        public string Job { get; set; }
        
        public bool IsHealthy { get; set; }
        
        public bool IsFriend { get; set;  }
        
        public bool IsTrusted { get; set; }
        
        public List<string> MedicalHistory { get; set; }
        
        public DateTime LastNewsUpdateAbout { get; set; }
        
        public string Description { get; set; }
    }
}