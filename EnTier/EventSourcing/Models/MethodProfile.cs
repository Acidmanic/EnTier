using System;
using System.Reflection;

namespace EnTier.EventSourcing.Models
{
    public class MethodProfile
    {
        public MethodInfo Method { get; set; }
        
        public Type ModelType { get; set; }
        
        public string Name { get; set; }
        
    }
}