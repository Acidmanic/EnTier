using Acidmanic.Utilities.Filtering.Attributes;
using Acidmanic.Utilities.Reflection.Attributes;

namespace ExampleEntityFramework.StoragesModels
{
    public class PostStg
    {
        
        public long PostStgId { get; set; }
        [AutoValuedMember]
        [UniqueMember]
        public long Id { get; set; }
        [FilterField]
        public string Title { get; set; }
        [FilterField]
        public string Content { get; set; }
        
    }
}