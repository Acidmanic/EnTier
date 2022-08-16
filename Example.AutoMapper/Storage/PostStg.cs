using System;
using Acidmanic.Utilities.Reflection.Attributes;

namespace Example.AutoMapper.Storage
{
    public class PostStg
    {
        [AutoValuedMember]
        [UniqueMember]
        public Guid Id { get; set; }
        
        public string Title { get; set; }
        
        public string Content { get; set; }
        
    }
}