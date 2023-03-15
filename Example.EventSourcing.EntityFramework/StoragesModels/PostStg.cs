using Acidmanic.Utilities.Reflection.Attributes;

namespace Example.EventSourcing.EntityFramework.StoragesModels
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