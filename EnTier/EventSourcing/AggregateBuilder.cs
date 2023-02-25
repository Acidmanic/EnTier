using System;
using System.Linq;
using Acidmanic.Utilities.Reflection;
using Acidmanic.Utilities.Reflection.Extensions;

namespace EnTier.EventSourcing;


/// <summary>
/// Aggregates contain business logic, so they might need to consume business services so they should be able to use
/// constructor injections. 
/// </summary>
public class AggregateBuilder : IAggregateBuilder
{
    private readonly Func<Type, object> _resolver;


    public AggregateBuilder(Func<Type, object> resolver)
    {
        _resolver = resolver;
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
    
    
    public IAggregate<TAggregateRoot, TEvent, TStreamId> Build<TAggregateRoot, TEvent, TStreamId>()
    {
        var types = this.GetType().Assembly.GetAvailableTypes()
            .Where(Implements<IAggregate<TAggregateRoot, TEvent, TStreamId>>);

        foreach (var type in types)
        {
            var instance = _resolver.Invoke(type);
            // If Registered
            if (instance is IAggregate<TAggregateRoot, TEvent, TStreamId> aggregateResolved)
            {
                return aggregateResolved;
            }

            instance = new ObjectInstantiator().BlindInstantiate(type);
            
            if (instance is IAggregate<TAggregateRoot, TEvent, TStreamId> aggregateInstantiated)
            {
                return aggregateInstantiated;
            }
        }

        return new NullAggregate<TAggregateRoot, TEvent, TStreamId>();
    }
}