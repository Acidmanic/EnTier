using System;
using Acidmanic.Utilities.Filtering.Attributes;
using Acidmanic.Utilities.Reflection.Attributes;

namespace ExampleJsonFile.Domain
{
    public class Post
    {
        [AutoValuedMember]
        [UniqueMember]
        public long Id { get; set; }
        
        public string Title { get; set; }
        
        public string Content { get; set; }
        
    }
}