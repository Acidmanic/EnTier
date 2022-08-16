using System;
using Acidmanic.Utilities.Reflection.Attributes;

namespace ExampleRegulation.Models
{
    public class Post
    {
        [AutoValuedMember]
        [UniqueMember]
        public string Id { get; set; }
        
        public string Title { get; set; }
        
        public string Content { get; set; }
        
    }
}