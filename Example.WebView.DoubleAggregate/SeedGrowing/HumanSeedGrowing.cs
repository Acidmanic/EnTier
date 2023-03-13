

using System;
using EnTier.EventSourcing;
using Example.WebView.DoubleAggregate.HumanArea;

namespace Example.WebView.DoubleAggregate.SeedGrowing
{


    public class HumanSeedGrowing : SeedGrowingBase<Human,IHumanEvent,long,Guid>
    {
        public HumanSeedGrowing(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        protected override IAggregate<Human, IHumanEvent, Guid> SetupAggregate()
        {
            var ag = new HumanAggregate();
            
            ag.Register(Gibber(1),Gibber(2),Gibber(3));

            return ag;
        }
    }
}
