using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace EnTier.Utility
{
    internal class CachedReflection : Reflection
    {
        private readonly List<MetaType> _types;

        public CachedReflection()
        {
            _types = new List<MetaType>();
        }


        private void AddRange(ICollection<Type> types)
        {
            types.ToList().ForEach(t => _types.Add(new MetaType(t)));
        }


        public CachedReflection CacheCurrent()
        {
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();

            foreach (var ass in assemblies)
            {
                Cache(ass);
            }

            //Cache(Assembly.GetExecutingAssembly());

            return this;
        }

        internal List<MetaType> All()
        {
            return _types.Where(FilterPredicate).ToList();
        }

        public CachedReflection Cache(Assembly assembly)
        {
            AddRange(assembly.GetTypes());
            return this;
        }

        internal void Cache(Type type)
        {
            _types.Add(new MetaType(type));
        }

        public CachedReflection CachePropertiesOf(Object obj)
        {
            var properties = obj.GetType().GetProperties();

            foreach (var prop in properties)
            {
                if (prop.CanRead)
                {
                    _types.Add(new MetaType(
                        prop.PropertyType,
                        () => prop.GetValue(obj)
                    ));
                }
            }

            return this;
        }

        public CachedReflection CachePropertyTypes(Object obj)
        {
            var properties = obj.GetType().GetProperties();

            foreach (var prop in properties)
            {
                _types.Add(new MetaType(prop.PropertyType));
            }

            return this;
        }


        public List<Type> GetTypesWhichImplement<T>()
        {
            var type = typeof(T);

            return GetTypesWhichImplement(type);
        }

        public List<Type> GetTypesWhichImplement(Type type)
        {
            return _types.Where(FilterPredicate).ToList()
                .Select(t => t.Type)
                .Where(t => Implements(t, type))
                .ToList();
        }


        public List<Type> GetTypesWhichExtend<T>()
        {
            var type = typeof(T);

            return GetTypesWhichExtend(type);
        }

        public List<Type> GetTypesWhichExtend(Type type)
        {
            return _types.Where(FilterPredicate).ToList()
                .Select(t => t.Type)
                .Where(t => Extends(t, type))
                .ToList();
        }


        public bool IsAnyExtensionFor<T>()
        {
            var type = typeof(T);

            return _types.Where(FilterPredicate).ToList()
                .Select(t => t.Type)
                .Where(t => Extends(t, type))
                .Count() > 0;
        }


        public bool IsAnyImplementationFor<T>()
        {
            var type = typeof(T);

            return _types.Where(FilterPredicate).ToList()
                .Select(t => t.Type)
                .Where(t => Implements(t, type))
                .Count() > 0;
        }

        public Constructor<TCast> GetCreatorForTypeWhichImplements<TCast>()
        {
            var type = typeof(TCast);

            return GetCreatorForTypeWhich<TCast>(
                mt => Implements(mt.Type, type)
            );
        }

        public Constructor<TCast> GetCreatorForTypeWhichExtends<TCast>()
        {
            var type = typeof(TCast);

            return GetCreatorForTypeWhich<TCast>(
                mt => Extends(mt.Type, type)
            );
        }

        public List<Constructor<TCast>> GetCreatorsWhich<TCast>(Func<MetaType, bool> predicate)
        {
            var ret = new List<Constructor<TCast>>();

            _types.Where(FilterPredicate).ToList()
                .Where(predicate)
                .Select(mt => mt.Instanciator)
                .ToList()
                .ForEach(f => ret.Add(new Constructor<TCast>(() => (TCast) f())));

            return ret;
        }

        public Constructor<TCast> GetCreatorForTypeWhich<TCast>(Func<MetaType, bool> predicate)
        {
            var res = _types.Where(FilterPredicate).ToList()
                .Where(predicate)
                .Select(mt => mt.Instanciator)
                .FirstOrDefault();

            if (res != null)
            {
                return new Constructor<TCast>(() => (TCast) res());
            }

            return Constructor<TCast>.Null();
        }

        public Type[] GetTypesArrayForObjects(params Object[] objects)
        {
            var ret = new Type[objects.Length];

            for (int i = 0; i < objects.Length; i++)
            {
                ret[i] = objects[i].GetType();
            }

            return ret;
        }

        public Constructor<TCast> FindConstructor<TCast>(
            Func<Type, bool> predicate,
            params Object[] arguments
        )
        {
            var creators = _types.Where(FilterPredicate).ToList()
                .FindAll(mt => predicate(mt.Type));

            if (creators != null)
            {
                foreach (var c in creators)
                {
                    var res = GetConstructorForType<TCast>(c.Type, arguments);

                    if (!res.IsNull) return res;
                }
            }

            return Constructor<TCast>.Null();
        }

        public List<Constructor<TCast>> FindConstructors<TCast>(
            Func<Type, bool> predicate,
            params Object[] arguments
        )
        {
            var creators = _types.Where(FilterPredicate).ToList()
                .FindAll(mt => predicate(mt.Type));

            var ret = new List<Constructor<TCast>>();

            if (creators != null)
            {
                foreach (var c in creators)
                {
                    var res = GetConstructorForType<TCast>(c.Type, arguments);

                    if (!res.IsNull) ret.Add(res);
                }
            }

            return ret;
        }

        public Constructor<object> GetConstructorForType(Type type, params object[] arguments)
        {
            return GetConstructorForType<object>(type, arguments);
        }

        public Constructor<TCast> GetConstructorForType<TCast>(Type type, params object[] arguments)
        {
            if (!type.IsAbstract && !type.IsInterface)
            {
                var argTypes = GetTypesArrayForObjects(arguments);

                var constructor = type.GetConstructor(argTypes);

                if (constructor != null)
                {
                    return new Constructor<TCast>(constructor, arguments);
                }
            }
            return Constructor<TCast>.Null();
        }

        public Constructor<TCast> FindConstructor<TCast>(params Object[] argumets)
        {
            var type = typeof(TCast);

            return FindConstructor<TCast>(
                t => Implements(t, type)
                , argumets
            );
        }

        public Constructor<TInterface> FindConstructorByPriority<TInterface, TLowPrType>(params Object[] arguments)
        {
            var type = typeof(TInterface);
            var exclude = typeof(TLowPrType);
            var ret = FindConstructor<TInterface>(
                t => t != exclude && Implements(t, type)
                , arguments
            );

            if (ret.IsNull)
            {
                ret = GetConstructorForType<TInterface>(exclude, arguments);
            }

            return ret;
        }

        public bool Implements<TInterface, TType>()
        {
            return Implements<TInterface>(typeof(TType));
        }

        public bool Implements<TInterface>(Type type)
        {
            return Implements(type, typeof(TInterface));
        }

        public bool Implements(Type type, Type @interface)
        {
            var interfaces = type.GetInterfaces();

            foreach (var i in interfaces)
            {
                if (i.Equals(@interface)) return true;
            }

            var parent = type.BaseType;

            if (parent != null) return Implements(parent, @interface);

            return false;
        }

        public bool Extends<TBase>(Type t)
        {
            var @base = typeof(TBase);

            return Extends(t, @base);
        }

        public bool IsSpecificOf<TGeneric>(Type specific)
        {
            return IsSpecificOf(specific, typeof(TGeneric));
        }

        public bool IsSpecificOf(Type specific, Type generic)
        {
            return specific != null &&
                   (
                       (specific.IsGenericType && specific.GetGenericTypeDefinition() == generic)
                       ||
                       IsSpecificOf(specific.BaseType, generic)
                   );
        }

        public bool Extends(Type t, Type @base)
        {
            if (t == @base) return true;

            var parent = t.BaseType;

            if (parent != null)
            {
                return Extends(parent, @base);
            }

            return false;
        }

        public Type GetInterfaceWithAttribute<TAttribute>(Type type)
        {
            var interfaces = type.GetInterfaces();
            var attType = typeof(TAttribute);
            foreach (var i in interfaces)
            {
                var att = i.GetCustomAttribute(attType, false);

                if (att != null)
                {
                    return i;
                }
            }

            return null;
        }

        private readonly List<Func<MetaType, bool>> _filters = new List<Func<MetaType, bool>>();

        public CachedReflection Filter(Func<MetaType, bool> filter)
        {
            _filters.Add(filter);

            return this;
        }

        public CachedReflection ClearFilters()
        {
            _filters.Clear();

            return this;
        }

        public CachedReflection FilterRemoveImplementers<T>()
        {
            var iface = typeof(T);

            _filters.Add(t => !Implements(t.Type, iface));

            return this;
        }

        public CachedReflection FilterAlloweImplementers<T>()
        {
            var iface = typeof(T);

            _filters.Add(t => Implements(t.Type, iface));

            return this;
        }

        internal CachedReflection FillterAllowAssignables<TInterface>()
        {
            var iface = typeof(TInterface);

            _filters.Add(t =>
                iface.IsAssignableFrom(t.Type)
            );


            return this;
        }


        internal List<MetaType> Debug()
        {
            return _types;
        }


        protected Func<MetaType, bool> FilterPredicate
        {
            get
            {
                return (t) =>
                {
                    foreach (var f in _filters)
                    {
                        if (!f(t)) return false;
                    }

                    return true;
                };
            }
        }

        public Type GetAncesstor(Type type, params Func<Type, bool>[] conditions)
        {
            var ancesstors = GetAncesstors(type);

            bool condition(Type t)
            {
                foreach (var c in conditions)
                {
                    if (!c(t)) return false;
                }

                return true;
            }

            foreach (var anc in ancesstors)
            {
                if (condition(anc))
                {
                    return anc;
                }
            }

            return null;
        }

        private List<Type> GetAncesstors(Type t)
        {
            var ret = new List<Type>();

            var parent = t;

            while (parent != null)
            {
                ret.Add(parent);

                parent = parent.BaseType;
            }

            return ret;
        }
    }
}