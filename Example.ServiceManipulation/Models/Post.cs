using Acidmanic.Utilities.Reflection.Attributes;

namespace ServiceManipulationExample.Models
{
    public class Post
    {
        [AutoValuedMember]
        [UniqueMember]
        public long Id { get; set; }
        
        public string Title { get; set; }
        
        public string Content { get; set; }
        
    }
}