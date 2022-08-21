using System;
using EnTier.Repositories;
using EnTier.TestByCase.Models;

namespace EnTier.TestByCase.Fixtures
{
    public class PropertyTypeDalFixture
    {


        public void Setup(ICrudRepository<PropertyTypeDal, long> repository)
        {
            Console.WriteLine($"Fixture has been executed with: {repository}");
        }
    }
}