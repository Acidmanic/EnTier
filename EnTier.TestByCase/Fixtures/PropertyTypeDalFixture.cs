using System;
using EnTier.Repositories;
using EnTier.TestByCase.Models;

namespace EnTier.TestByCase.Fixtures
{
    public class PropertyTypeDalFixture
    {
        public static PropertyTypeDal FirstType = new PropertyTypeDal
        {
            Id = 0,
            Name = "Pashang",
            Value = "Mashang"
        };

        public void Setup(ICrudRepository<PropertyTypeDal, long> repository)
        {
            var inserted = repository.Add(FirstType);

            FirstType.Id = inserted.Id;

        }
    }
}