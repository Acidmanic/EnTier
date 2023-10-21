using System;
using Acidmanic.Utilities.Filtering.Attributes;
using Acidmanic.Utilities.Reflection.Attributes;

namespace ExampleInMemorySingleLayerEntity.Models
{
    public class Post
    {
        [AutoValuedMember]
        [UniqueMember]
        public string Id { get; set; }
        
        [FilterField]
        public string Title { get; set; }
        
        public string Content { get; set; }
        
    }
}