using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Acidmanic.Utilities.Reflection;
using Acidmanic.Utilities.Reflection.Extensions;
using EnTier.Extensions;
using Microsoft.Extensions.Logging;

namespace EnTier.EventSourcing
{
    /// <summary>
    /// Aggregates contain business logic, so they might need to consume business services so they should be able to use
    /// constructor injections. 
    /// </summary>
    public class AggregateBuilder : IAggregateBuilder
    {
        private readonly Func<Type, object> _resolver;

        private readonly List<Assembly> _assemblies = new List<Assembly>();

        private ILogger Logger { get; }

        public AggregateBuilder(Func<Type, object> resolver, ILogger logger, params Assembly[] assemblies)
        {
            _resolver = resolver;

            Logger = logger;

            _assemblies.AddRange(assemblies);
        }

        private bool Implements<T>(Type type)
        {
            var interfaces = type.GetInterfaces();

            var abstraction = typeof(T);

            foreach (var @interface in interfaces)
            {
                if (@interface == abstraction)
                {
                    return true;
                }
            }

            return false;
        }


        public Type FindAggregateType<TAggregateRoot, TEvent, TStreamId>()
        {
            return LoadTypes<TAggregateRoot, TEvent, TStreamId>().FirstOrDefault();
        }

        private List<Type> LoadTypes<TAggregateRoot, TEvent, TStreamId>()
        {
            var types = new List<Type>();

            foreach (var assembly in _assemblies)
            {
                var adding = assembly.GetAvailableTypes()
                    .Where(Implements<IAggregate<TAggregateRoot, TEvent, TStreamId>>);

                types.AddRange(adding);
            }

            return types;
        }


        public IAggregate<TAggregateRoot, TEvent, TStreamId> Build<TAggregateRoot, TEvent, TStreamId>()
        {
            var types = LoadTypes<TAggregateRoot, TEvent, TStreamId>();

            foreach (var type in types)
            {
                var instance = _resolver.Invoke(type);
                // If Registered
                if (instance is IAggregate<TAggregateRoot, TEvent, TStreamId> aggregateResolved)
                {
                    return aggregateResolved;
                }

                if (type.IsNewable())
                {
                    instance = new ObjectInstantiator().BlindInstantiate(type);

                    if (instance is IAggregate<TAggregateRoot, TEvent, TStreamId> aggregateInstantiated)
                    {
                        return aggregateInstantiated;
                    }
                }
                else
                {
                    Logger.LogError(
                        "Un-Registered Dependant Aggregate\nIf Your aggregate implementation has constructor injected dependencies, " +
                        "you need to register the implementation in your Di, which is introduced to " +
                        "AggregateBuilder class using 'Func<Type,object> resolver' constructor argument.");
                }
            }

            return new NullAggregate<TAggregateRoot, TEvent, TStreamId>();
        }
    }
}