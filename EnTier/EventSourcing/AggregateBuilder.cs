using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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

    private readonly List<Assembly> _assemblies = new List<Assembly>();

    public AggregateBuilder(Func<Type, object> resolver, params Assembly[] assemblies)
    {
        _resolver = resolver;

        _assemblies.AddRange(assemblies);

        AddAss(Assembly.GetCallingAssembly());
        AddAss(Assembly.GetEntryAssembly());
        AddAss(Assembly.GetExecutingAssembly());
    }

    private void AddAss(Assembly? assembly)
    {
        if (assembly != null)
        {
            if (!_assemblies.Contains(assembly))
            {
                _assemblies.Add(assembly);
            }
        }
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


    public Type? FindAggregateType<TAggregateRoot, TEvent, TStreamId>()
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

            instance = new ObjectInstantiator().BlindInstantiate(type);

            if (instance is IAggregate<TAggregateRoot, TEvent, TStreamId> aggregateInstantiated)
            {
                return aggregateInstantiated;
            }
        }

        return new NullAggregate<TAggregateRoot, TEvent, TStreamId>();
    }
}