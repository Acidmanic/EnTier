using System;
using Acidmanic.Utilities.Reflection.Attributes;

namespace ExampleJsonFile.Dto
{
    public class PostDto
    {
        [AutoValuedMember]
        [UniqueMember]
        public long Id { get; set; }
        
        public string Title { get; set; }
        
        public string Content { get; set; }
        
    }
}