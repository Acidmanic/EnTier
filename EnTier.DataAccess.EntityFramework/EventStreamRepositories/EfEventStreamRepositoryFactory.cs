using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Acidmanic.Utilities.Factories;
using Acidmanic.Utilities.Reflection;
using Acidmanic.Utilities.Reflection.Extensions;
using Acidmanic.Utilities.Results;
using EnTier.Repositories;
using Microsoft.EntityFrameworkCore;

namespace EnTier.DataAccess.EntityFramework.EventStreamRepositories;

internal class RepositoryProfile
{
    public Type EventIdType { get; set; }

    public Type StreamIdType { get; set; }

    public Type RepositoryType { get; set; }

    public ConstructorInfo Constructor { get; set; }
}

internal class EfEventStreamRepositoryFactory
{
    private static readonly object Lock = new object();
    private static EfEventStreamRepositoryFactory _instance = null;


    public static EfEventStreamRepositoryFactory Instance
    {
        get
        {
            lock (Lock)
            {
                if (_instance == null)
                {
                    _instance = new EfEventStreamRepositoryFactory();
                }

                return _instance;
            }
        }
    }


    private readonly List<RepositoryProfile> _repositoryProfiles;

    private EfEventStreamRepositoryFactory()
    {
        _repositoryProfiles = new List<RepositoryProfile>();

        var assembly = this.GetType().Assembly;

        var types = assembly.GetAvailableTypes();

        foreach (var type in types)
        {
            var foundProfile = IsAnEsRepository(type);

            if (foundProfile)
            {
                _repositoryProfiles.Add(foundProfile);
            }
        }
    }

    private Result<RepositoryProfile> IsAnEsRepository(Type type)
    {
        if (type == null || type.IsAbstract || type.IsInterface)
        {
            return new Result<RepositoryProfile>().FailAndDefaultValue();
        }

        var abstractEventStream = typeof(IEventStreamRepository<,,>);

        var interfaces = type.GetInterfaces();

        foreach (var @interface in interfaces)
        {
            if (@interface.IsGenericType)
            {
                var genericEsIface = @interface.GetGenericTypeDefinition();

                if (genericEsIface == abstractEventStream)
                {
                    var parameters = @interface.GetGenericArguments();

                    if (parameters.Length == 3)
                    {
                        var constructor = type.GetConstructors().FirstOrDefault(IsCorrectDbSetArgumentConstructor);

                        if (constructor != null)
                        {
                            return new Result<RepositoryProfile>(true, new RepositoryProfile
                            {
                                RepositoryType = type,
                                EventIdType = parameters[1],
                                StreamIdType = parameters[2],
                                Constructor = constructor
                            });
                        }
                    }
                }
            }
        }

        return new Result<RepositoryProfile>().FailAndDefaultValue();
    }

    private bool IsCorrectDbSetArgumentConstructor(ConstructorInfo constructor)
    {
        var parameters = constructor.GetParameters();

        var abstractDbSetType = typeof(DbSet<>);

        if (parameters.Length == 1)
        {
            var parameterType = parameters[0].ParameterType;

            return TypeCheck.IsSpecificOf(parameterType, abstractDbSetType);
        }

        return false;
    }

    public IEventStreamRepository<TEvent, TEventId, TStreamId> Make<TEvent, TEventId, TStreamId>
        (DbSet<EfObjectEntry<TEventId, TStreamId>> dbSet)
    {
        var matchedProfile = _repositoryProfiles
            .Where(RepositoryMatches<TEvent, TEventId, TStreamId>)
            .FirstOrDefault(p => IsInstantiatableBy(p, dbSet));

        if (matchedProfile != null)
        {
            var repository = Instantiate<TEvent, TEventId, TStreamId>(matchedProfile, dbSet);

            if (repository != null)
            {
                return repository;
            }
        }

        return NullEfEventStreamRepository<TEvent, TEventId, TStreamId>.Instance;
    }

    private IEventStreamRepository<TEvent, TEventId, TStreamId> Instantiate<TEvent, TEventId, TStreamId>
        (RepositoryProfile profile, DbSet<EfObjectEntry<TEventId, TStreamId>> dbSet)
    {
        var genericType = profile.RepositoryType.GetGenericTypeDefinition();

        var concreteType = genericType.MakeGenericType(typeof(TEvent));

        var constructor = concreteType.GetConstructors().FirstOrDefault(IsCorrectDbSetArgumentConstructor);

        if (constructor != null)
        {
            try
            {
                var instantiated = constructor.Invoke(new object[] { dbSet });

                return instantiated as IEventStreamRepository<TEvent, TEventId, TStreamId>;
            }
            catch (Exception e)
            {
            } 
        }
        
        return null;
    }


    private bool RepositoryMatches<TEvent, TEventId, TStreamId>(RepositoryProfile profile)
    {
        return profile.EventIdType == typeof(TEventId) &&
               profile.StreamIdType == typeof(TStreamId);
    }

    private bool IsInstantiatableBy<TEventId, TStreamId>
        (RepositoryProfile profile, DbSet<EfObjectEntry<TEventId, TStreamId>> dbSet)
    {
        if (dbSet == null)
        {
            return false;
        }

        var parameterType = profile.Constructor.GetParameters()[0].ParameterType;

        return parameterType.IsInstanceOfType(dbSet);
    }
}