using System;
using EnTier.Fixture;
using EnTier.TestByCase.Fixtures;

namespace EnTier.TestByCase
{
    public class Tbc001IdGeneratorInFixture:TestBase
    {


        private class Resolver : IFixtureResolver
        {
            public T Resolve<T>()
            {
                return (T) Resolve(typeof(T));
            }

            public object Resolve(Type type)
            {
                return new PropertyTypeDalFixture();
            }
        }
        
        public override void Main()
        {
            var resolver = new Resolver();
            
            FixtureManager.UseFixture<PropertyTypeDalFixture>(resolver);
            
            
            
        }
    }
}