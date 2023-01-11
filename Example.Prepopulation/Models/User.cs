using Acidmanic.Utilities.Reflection.Attributes;

namespace Example.Prepopulation.Models
{
    public class User
    {
        [AutoValuedMember]
        [UniqueMember]
        public long Id { get; set; }
        
        public string FullName { get; set; }
        
        public string Username { get; set; }
        
        public string Email { get; set; }
        
    }
}