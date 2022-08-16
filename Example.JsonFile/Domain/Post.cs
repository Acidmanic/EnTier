using System;
using Acidmanic.Utilities.Reflection.Attributes;

namespace ExampleJsonFile.Domain
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