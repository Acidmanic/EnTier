using System;
using EnTier.Extensions;
using EnTier.Fixture;
using EnTier.Logging;
using EnTier.TestByCase.Fixtures;

namespace EnTier.TestByCase
{
    /// <summary>
    /// This case, investigates the issue occuring while using repository.insert within a fixture,
    ///  using in memory unitofwork
    /// </summary>
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
            
            new ConsoleLogger().UseLoggerForEnTier();
            
            var resolver = new Resolver();
            
            FixtureManager.UseFixture<PropertyTypeDalFixture>(resolver);
            
            
            
        }
    }
}