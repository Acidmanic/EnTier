using System.ComponentModel.DataAnnotations;
using System.Data.Common;
using Acidmanic.Utilities.Reflection.Attributes;

namespace ExampleEntityFramework.StoragesModels
{
    public class PostStg
    {
        
        public long PostStgId { get; set; }
        
        [AutoValuedMember]
        [UniqueMember]
        public long Id { get; set; }
        
        public string Title { get; set; }
        
        public string Content { get; set; }
        
    }
}