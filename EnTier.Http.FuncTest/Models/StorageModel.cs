using Acidmanic.Utilities.Reflection.Attributes;
using EnTier.Query.Attributes;

namespace EnTier.Http.FuncTest.Models
{
    public class StorageModel
    {
        [UniqueMember] [AutoValuedMember] public long Id { get; set; }

        [FilterField] public string Name { get; set; }
        [FilterField] public int Age { get; set; }
        [FilterField] public int Weight { get; set; }
        [FilterField] public string Surname { get; set; }
        [FilterField] public int Height { get; set; }
    }
}