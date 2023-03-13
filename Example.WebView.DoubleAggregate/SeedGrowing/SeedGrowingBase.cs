using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using EnTier.EventSourcing;
using EnTier.EventSourcing.Attributes;
using EnTier.Extensions;
using EnTier.Reflection;
using EnTier.UnitOfWork;

namespace Example.WebView.DoubleAggregate.SeedGrowing
{
    public abstract class SeedGrowingBase<TAggregateRoot,TEvent,TEventId,TStreamId>
    {
        private static readonly string[] Words =
        {
            "notebook", "hardship", "monster", "flush", "ethnic", "soft", "continuation",
            "emphasis", "country", "smash", "prove", "lineage", "depend", "gap", "agony", "inject", "directory",
            "morning", "shine", "venture", "sulphur", "nominate", "match", "duck", "function", "intention",
            "charity","drawer","climate","hat","bathroom","data","extent","replacement","success","foundation",
            "grocery","revolution","nature","stranger","passion","hearing","society","funeral","bread","chocolate",
            "estate","homework","ladder","member","volume","football","teacher","aspect","length","protection","cookie",
            "platform","disk","error","library","news","disease","family","youth","selection","meal","menu","contract",
            "relation","speaker","desk","boyfriend","scene","apartment","clammy","southern","existing","heady",
            "tedious","peaceful","polite","squealing","stereotyped","mean","common","tired","sorry","painful",
            "impolite","abundant","future","entire","helpful","resolute","probable","tenuous","selective","fragile",
            "rigid","obsequious","elfin","quarrelsome","tall","organic","wide","remarkable","macho","well-made",
            "legal","alleged","holistic","oceanic","distinct","striped","unwieldy","uppity","righteous","adaptable",
            "minor","homeless","unhappy","emotional","vague","waggish"
        };

        private static readonly Random Random = new Random();
        
        private readonly IServiceProvider _serviceProvider;

        public SeedGrowingBase(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }


        protected int[] Randex(int count,int maxClude)
        {
            int[] randomIndex = new int[count];

            for (int i = 0; i < count; i++)
            {
                randomIndex[i] = Random.Next(0, maxClude);
            }

            return randomIndex;
        }

        protected string GibberOrDefault(int count)
        {
            if (Random.Next(0, 2) < 1)
            {
                return null;
            }

            return Gibber(count);
        }

        protected string Gibber(int count)
        {
            
            var randex = Randex(count,Words.Length);

            var words = randex.Select(i => Words[i]);

            var gibber = string.Join(' ', words);

            return gibber;
        }


        protected abstract IAggregate<TAggregateRoot, TEvent, TStreamId> SetupAggregate();
        
        public Task GrowHumans(int numberOfHumans, int numberOfActions)
        {
            var unitOfWork = _serviceProvider.GetService<IUnitOfWork>();
        
            var repository = unitOfWork.GetStreamRepository<TEvent,TEventId,TStreamId>();
        
        
            for (int number = 0; number < numberOfHumans; number++)
            {
                var hAggregate = SetupAggregate();

                for (int action = 0; action < numberOfActions; action++)
                {
                    DoOneRandomSheet(hAggregate);
                }
        
                async void Save(TEvent ev) => await repository.Append(ev, hAggregate.StreamId);
        
                hAggregate.Updates.ForEach(Save);
            }
            
            unitOfWork.Complete();
        
            return Task.CompletedTask;
        }


        private bool IsGoodRandomShitMethod(MethodInfo methodInfo)
        {
            return methodInfo.GetCustomAttribute<NoStreamIdApi>() == null;
        }
        
        private void DoOneRandomSheet(IAggregate<TAggregateRoot, TEvent, TStreamId> hAggregate)
        {
            var done = false;

            var index = new AggregateIndex(hAggregate.GetType());

            var profiles = index.MethodProfiles.
                Values.Where(p => IsGoodRandomShitMethod(p.Method)).ToList();
            
            while (!done)
            {
                var action = Random.Next(0, profiles.Count);

                try
                {
                    var method = profiles[action];

                    PerformGiberrian(hAggregate, method.Method);

                    done = true;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Random Shit Exception: {ex.Message}" );
                }
            }
        }

        private void PerformGiberrian(object owner, MethodInfo method)
        {

            var values = new Dictionary<string, object>();

            var parameters = method.GetParameters();

            foreach (var parameter in parameters)
            {
                if (!string.IsNullOrEmpty(parameter.Name))
                {
                    var value = Gibber(parameter.ParameterType);
                
                    values.Add(parameter.Name,value);    
                }
            }
            
            var execution = new MethodExecute();

            var result = execution.Execute(owner, method, values);

            if (!result.Successful)
            {
                throw new Exception("Try Again!");
            }
        }


        protected object Gibber(Type type)
        {
            if (type == typeof(bool))
            {
                return Random.Next(0, 1) == 0;
            }

            if (type == typeof(long) || type == typeof(int))
            {
                return Random.Next(0, int.MaxValue);
            }

            if (type == typeof(string))
            {
                var count = Random.Next(1, 10);

                return Gibber(count);
            }

            if (type == typeof(char))
            {
                return (char)Random.Next((int)'a', (int)'z');
            }

            if (type == typeof(byte))
            {
                return Random.Next(byte.MinValue, byte.MaxValue);
            }

            if (type == typeof(Guid))
            {
                return Guid.NewGuid();
            }

            if (type == typeof(decimal) || type == typeof(float) ||type == typeof(double))
            {
                return (Random.NextDouble() -0.5)* 10000000000;
            }

            return default;
        }
    }
}