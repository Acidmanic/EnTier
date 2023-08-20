using EnTier.Repositories;
using ExampleInMemorySingleLayerEntity.Models;

namespace ExampleInMemorySingleLayerEntity
{
    public class ProductsSeedFixture
    {
        public void Setup(ICrudRepository<Product, long> repository)
        {
            repository.Add(new Product
            {
                Name = "First",
                Description = "First Product",
                Rating = 0.2,
                Weight = 10
            });

            repository.Add(new Product
            {
                Name = "Second",
                Description = "Second Product",
                Rating = 0.4,
                Weight = 20
            });

            repository.Add(new Product
            {
                Name = "Third",
                Description = "Third Product",
                Rating = 0.6,
                Weight = 30
            });

            repository.Add(new Product
            {
                Name = "Fourth",
                Description = "Fourth Product",
                Rating = 0.8,
                Weight = 40
            });

            repository.Add(new Product
            {
                Name = "Fifth",
                Description = "Fifth Product",
                Rating = 1,
                Weight = 50
            });
        }
    }
}