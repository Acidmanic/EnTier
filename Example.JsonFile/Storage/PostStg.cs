using System;
using Acidmanic.Utilities.Reflection.Attributes;

namespace ExampleJsonFile.Storage
{
    public class PostStg
    {
        [AutoValuedMember]
        [UniqueMember]
        public string Id { get; set; }
        
        public string Title { get; set; }
        
        public string Content { get; set; }
        
    }
}