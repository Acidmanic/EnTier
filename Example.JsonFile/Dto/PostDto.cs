using System;
using Acidmanic.Utilities.Reflection.Attributes;

namespace ExampleJsonFile.Dto
{
    public class PostDto
    {
        [AutoValuedMember]
        [UniqueMember]
        public string Id { get; set; }
        
        public string Title { get; set; }
        
        public string Content { get; set; }
        
    }
}