using Acidmanic.Utilities.Reflection.Attributes;

namespace Example.EventSourcing.EntityFramework.TransferModels
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