

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


// using System;
// using System.Linq;
// using System.Threading.Tasks;
// using EnTier.Extensions;
// using EnTier.UnitOfWork;
// using Example.WebView.DoubleAggregate.HumanArea;
//
// namespace Example.WebView.DoubleAggregate.SeedGrowing
// {
//     public class HumanSeedGrowing
//     {
//         private static readonly string[] Words =
//         {
//             "notebook", "hardship", "monster", "flush", "ethnic", "soft", "continuation",
//             "emphasis", "country", "smash", "prove", "lineage", "depend", "gap", "agony", "inject", "directory",
//             "morning", "shine", "venture", "sulphur", "nominate", "match", "duck", "function"
//         };
//
//         private static readonly Random Random = new Random();
//
//         private readonly IServiceProvider _serviceProvider;
//
//         public HumanSeedGrowing(IServiceProvider serviceProvider)
//         {
//             _serviceProvider = serviceProvider;
//         }
//
//
//         private static int[] Randex(int count)
//         {
//             int[] randomIndex = new int[count];
//
//             for (int i = 0; i < count; i++)
//             {
//                 randomIndex[i] = Random.Next(0, count);
//             }
//
//             return randomIndex;
//         }
//
//         private static string GibberOrDefault(int count)
//         {
//             if (Random.Next(0, 2) < 1)
//             {
//                 return null;
//             }
//
//             return Gibber(count);
//         }
//
//         private static string Gibber(int count)
//         {
//             if (count > Words.Length)
//             {
//                 count = Words.Length;
//             }
//
//             var randex = Randex(count);
//
//             var words = randex.Select(i => Words[i]);
//
//             var gibber = string.Join(' ', words);
//
//             return gibber;
//         }
//
//
//         public Task GrowHumans(int numberOfHumans, int numberOfActions)
//         {
//             var unitOfWork = _serviceProvider.GetService<IUnitOfWork>();
//
//             var repository = unitOfWork.GetStreamRepository<IHumanEvent, long, Guid>();
//
//
//             for (int number = 0; number < numberOfHumans; number++)
//             {
//                 var hAggregate = new HumanAggregate();
//
//                 hAggregate.Register(Gibber(1), Gibber(2), Gibber(3));
//
//                 for (int action = 0; action < numberOfActions; action++)
//                 {
//                     DoRandomSheet(hAggregate);
//                 }
//
//                 async void Save(IHumanEvent ev) => await repository.Append(ev, hAggregate.StreamId);
//
//                 hAggregate.Updates.ForEach(Save);
//             }
//             
//             unitOfWork.Complete();
//
//             return Task.CompletedTask;
//         }
//
//         private void DoRandomSheet(HumanAggregate hAggregate)
//         {
//             var done = false;
//
//             while (!done)
//             {
//                 var action = Random.Next(0, 3);
//
//                 try
//                 {
//                     switch (action)
//                     {
//                         case 0:
//                             hAggregate.KnowCapabilities(Gibber(1), Gibber(1));
//                             break;
//                         case 1:
//                             hAggregate.KnowTaste(Gibber(2), Gibber(1));
//                             break;
//                         case 2:
//                             hAggregate.SetSickness(GibberOrDefault(4));
//                             break;
//                     }
//
//                     done = true;
//                 }
//                 catch (Exception) { }
//             }
//         }
//     }
// }