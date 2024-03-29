using System;
using EnTier.DependencyInjection;
using EnTier.Fixture;
using EnTier.TestByCase.Fixtures;
using Microsoft.Extensions.Logging.LightWeight;

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

            var container = new LiteContainer();
            
            var essence = new EnTierEssence().UseResolver(container);

            container.Register<EnTierEssence>(essence);
            
            FixtureManager.UseFixture<PropertyTypeDalFixture>(essence);
            
        }
    }
}