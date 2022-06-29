using Acidmanic.Utilities.Reflection.Attributes;

namespace Example.Meadow.Models
{
    public class Post
    {
        [UniqueMember]
        [AutoValuedMember]
        public long Id { get; set; }
        
        public string Title { get; set; }
        
        public string Content { get; set; }
        
    }
}