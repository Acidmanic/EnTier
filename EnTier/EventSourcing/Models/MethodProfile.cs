using System;
using System.Net.Http;
using System.Reflection;

namespace EnTier.EventSourcing.Models
{
    public class MethodProfile
    {
        public MethodInfo Method { get; set; }
        
        public Type ModelType { get; set; }
        
        public string Name { get; set; }
        
        public HttpMethod HttpMethod { get; set; }
        
        public bool NeedsStreamId { get; set; }
        
    }
}