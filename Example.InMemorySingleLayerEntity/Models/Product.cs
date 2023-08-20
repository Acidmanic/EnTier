using Acidmanic.Utilities.Reflection.Attributes;
using EnTier.Query.Attributes;

namespace ExampleInMemorySingleLayerEntity.Models
{
    public class Product
    {
        [AutoValuedMember] [UniqueMember] public long Id { get; set; }

        [FilterField] public string Name { get; set; }

        [FilterField] public string Description { get; set; }

        [FilterField] public double Rating { get; set; }

        [FilterField] public int Weight { get; set; }
    }
}