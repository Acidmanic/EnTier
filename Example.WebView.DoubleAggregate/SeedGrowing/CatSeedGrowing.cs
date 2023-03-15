using System;
using EnTier.EventSourcing;
using Example.WebView.DoubleAggregate.CatArea;

namespace Example.WebView.DoubleAggregate.SeedGrowing
{
    public class CatSeedGrowing:SeedGrowingBase<Cat,ICatEvent,long,long>
    {
        public CatSeedGrowing(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        protected override IAggregate<Cat, ICatEvent, long> SetupAggregate()
        {
            var ag = new CatAggregate();
            
            ag.Adopt(Gibber(1),3.0);

            return ag;
        }
    }
}