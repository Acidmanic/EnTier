using System.ComponentModel.DataAnnotations;
using Acidmanic.Utilities.Reflection.Attributes;

namespace EnTier.TestByCase.Models
{
    public class PropertyTypeDal
    {
        public string Name { get; set; }

        public string Value { get; set; }

        [Key]
        [UniqueMember]
        [AutoValuedMember]
        public long Id { get; set; }
    }
}