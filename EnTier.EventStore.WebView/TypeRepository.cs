using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Acidmanic.Utilities.Reflection;
using Acidmanic.Utilities.Reflection.Extensions;
using Acidmanic.Utilities.Results;
using EnTier.EventSourcing;
using EnTier.EventSourcing.Attributes;

namespace EnTier.EventStore.WebView
{
    internal class TypeRepository
    {
        private static TypeRepository _instance = null;
        private static readonly object InstantiationLock = new object();

        public static TypeRepository Instance
        {
            get
            {
                lock (InstantiationLock)
                {
                    if (_instance == null)
                    {
                        _instance = new TypeRepository();
                    }

                    return _instance;
                }
            }
        }

        private TypeRepository()
        {
            _stateClear = false;
        }

        private List<Assembly> Assemblies { get; } = new List<Assembly>();

        private Dictionary<Type, Type> _eventIdTypesByEventId = new Dictionary<Type, Type>();

        private bool _stateClear;
        private object _stateLock = new object();

        private readonly List<EventStreamProfile> _profiles = new List<EventStreamProfile>();

        public IReadOnlyList<EventStreamProfile> Profiles
        {
            get
            {
                ScanCheck();

                return _profiles;
            }
        }

        public TypeRepository Scan(Assembly assembly)
        {
            lock (_stateLock)
            {
                Assemblies.Add(assembly);

                _stateClear = false;

                return this;
            }
        }

        public TypeRepository RegisterEventIdType<TEvent, TEventId>()
        {
            var eventType = typeof(TEvent);
            var eventIdType = typeof(TEventId);

            if (_eventIdTypesByEventId.ContainsKey(eventType))
            {
                _eventIdTypesByEventId.Remove(eventType);
            }

            _eventIdTypesByEventId.Add(eventType, eventIdType);

            return this;
        }


        protected void ScanCheck()
        {
            lock (_stateLock)
            {
                if (_stateClear)
                {
                    return;
                }
            }

            _profiles.Clear();

            foreach (var assembly in Assemblies)
            {
                var types = assembly.GetAvailableTypes();

                var aggregateTypes =
                    types.Select(IsAggregateInterface)
                        .Where(r => r.Success);

                foreach (var type in aggregateTypes)
                {
                    var gotProfile = GetProfile(type.Primary, type.Secondary);

                    if (gotProfile)
                    {
                        _profiles.Add(gotProfile);
                    }
                }
            }

            lock (_stateLock)
            {
                _stateClear = true;
            }
        }

        private Result<EventStreamProfile> GetProfile(Type aggregateType, Type aggregateInterfaceType)
        {
            if (aggregateInterfaceType.IsGenericType)
            {
                var genericArguments = aggregateInterfaceType.GetGenericArguments();

                if (genericArguments.Length == 3)
                {
                    var eventIdType = GetEventIdType(genericArguments[1]);

                    var profile = new EventStreamProfile
                    {
                        AggregateType = aggregateType,
                        EventType = genericArguments[1],
                        AggregateRootType = genericArguments[0],
                        StreamIdType = genericArguments[2],
                        EventIdType = eventIdType
                    };

                    return new Result<EventStreamProfile>(true, profile);
                }
            }

            return new Result<EventStreamProfile>().FailAndDefaultValue();
        }

        private Type GetEventIdType(Type type)
        {
            if (_eventIdTypesByEventId.ContainsKey(type))
            {
                return _eventIdTypesByEventId[type];
            }

            var evIdTypeAttribute = type.GetCustomAttribute<EventStoreIdentifierAttribute>();

            if (evIdTypeAttribute != null)
            {
                return evIdTypeAttribute.EventIdType;
            }

            return typeof(long);
        }

        private Result<Type, Type> IsAggregateInterface(Type type)
        {
            if (!type.IsAbstract)
            {
                var genericType = typeof(IAggregate<,,>);

                var interfaces = type.GetInterfaces();

                foreach (var @interface in interfaces)
                {
                    if (TypeCheck.IsSpecificOf(@interface, genericType))
                    {
                        return new Result<Type, Type>(true, @interface, type);
                    }
                }
            }

            return new Result<Type, Type>().FailAndDefaultBothValues();
        }

        public Result<EventStreamProfile> ProfileByStreamName(string streamName)
        {
            var profile = Profiles.FirstOrDefault
                (p => p.EventType.Name.ToLower() == streamName.ToLower());

            return new Result<EventStreamProfile>(profile != null, profile);
        }
    }
}