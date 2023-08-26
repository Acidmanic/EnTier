using System;
using Acidmanic.Utilities.Filtering.Attributes;
using Acidmanic.Utilities.Reflection.Attributes;

namespace ExampleJsonFile.Storage
{
    public class PostStg
    {
        [AutoValuedMember]
        [UniqueMember]
        public long Id { get; set; }
        
        [FilterField] public string Title { get; set; }
        
        [FilterField] public string Content { get; set; }
        
    }
}