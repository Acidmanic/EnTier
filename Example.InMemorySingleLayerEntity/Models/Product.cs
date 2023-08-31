using Acidmanic.Utilities.Filtering.Attributes;
using Acidmanic.Utilities.Reflection.Attributes;
using EnTier.Attributes;

namespace ExampleInMemorySingleLayerEntity.Models
{
    public class Product
    {
        [AutoValuedMember] [UniqueMember] public long Id { get; set; }

        [FilterByExistingValues]
        [FilterField] public string Name { get; set; }

        [FilterField] public string Description { get; set; }

        [FilterField] public double Rating { get; set; }

        [FilterField] public int Weight { get; set; }
    }
}