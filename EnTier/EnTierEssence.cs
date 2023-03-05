using System;
using System.Collections.Generic;
using System.Reflection;
using EnTier.DataAccess.InMemory;
using EnTier.DependencyInjection;
using EnTier.EventSourcing;
using EnTier.Mapper;
using EnTier.Regulation;
using EnTier.UnitOfWork;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.LightWeight;

namespace EnTier
{
    /// <summary>
    /// This class will represent an instance of whole EnTier Library
    /// </summary>
    public class EnTierEssence
    {
        internal EnTierResolver Resolver { get; private set; }
        internal ILogger Logger { get; private set; }

        internal List<Assembly> AvailableAssemblies { get; private set; } = new List<Assembly>();

        public EnTierEssence(params Assembly[] additionalAssemblies)
        {
            Logger = new ConsoleLogger();

            Resolver = new NullEnTierResolver();

            AddAssembly(Assembly.GetCallingAssembly());
            AddAssembly(Assembly.GetEntryAssembly());
            AddAssembly(Assembly.GetExecutingAssembly());

            foreach (var assembly in additionalAssemblies)
            {
                AddAssembly(assembly);
            }
        }

        private void AddAssembly(Assembly assembly)
        {
            if (assembly != null)
            {
                if (!AvailableAssemblies.Contains(assembly))
                {
                    AvailableAssemblies.Add(assembly);
                }
            }
        }

        public EnTierEssence UseResolver(IResolverFacade resolver)
        {
            Resolver = new EnTierResolver(resolver);

            Logger = ResolveOrDefault<ILogger>(new ConsoleLogger());

            return this;
        }

        public EnTierEssence UseResolver(Func<Type, object> resolver)
        {
            return UseResolver(new ResolverAdapter(resolver));
        }


        internal T ResolveOrDefault<T>(object defaultValue, bool shootError = false) where T : class
        {
            return ResolveOrDefault<T>(() => defaultValue, shootError);
        }

        internal T ResolveOrDefault<T>(Func<object> defaultFactory, bool shootError = false) where T : class
        {
            return ResolveOrDefault(typeof(T), defaultFactory, shootError) as T;
        }

        internal object ResolveOrDefault(Type type, Func<object> defaultFactory, bool shootError = false)
        {
            var resolved = Resolver.TryResolve(type);

            if (resolved)
            {
                return resolved.Primary;
            }

            if (shootError)
            {
                Logger.LogError("You must register your implementation of choice for {TypeName}.", type.Name);
                if (resolved.Secondary != null)
                {
                    Logger.LogError(resolved.Secondary, "Exception: {Exception}", resolved.Secondary);
                }
            }

            return defaultFactory();
        }

        internal IUnitOfWork UnitOfWork => ResolveOrDefault<IUnitOfWork>(new InMemoryUnitOfWork(this));

        internal IMapper Mapper => ResolveOrDefault<IMapper>(new EntierBuiltinMapper());

        internal IDataAccessRegulator<TDomain, TStorage> Regulator<TDomain, TStorage>()
        {
            return ResolveOrDefault<IDataAccessRegulator<TDomain, TStorage>>
                (new NullDataAccessRegulator<TDomain, TStorage>());
        }

        internal IAggregateBuilder AggregateBuilder => ResolveOrDefault<IAggregateBuilder>
            (new AggregateBuilder(Resolver.Resolve, Logger, AvailableAssemblies.ToArray()));
    }
}