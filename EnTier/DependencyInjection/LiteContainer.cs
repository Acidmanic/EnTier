using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Acidmanic.Utilities.Results;
using EnTier.Prepopulation;

namespace EnTier.DependencyInjection
{
    public class LiteContainer : IServiceResolver,IResolverFacade
    {
        private readonly Dictionary<Type, Type> _registered = new Dictionary<Type, Type>();
        private readonly Dictionary<Type,Func<object>> _factories = new Dictionary<Type, Func<object>>();

        private class ConstructorComparer : IComparer<ConstructorInfo>
        {
            public int Compare(ConstructorInfo x, ConstructorInfo y)
            {
                if (x == null || y == null)
                {
                    throw new Exception("Constructor can not be null. How did even you get here?");
                }

                return x.GetParameters().Length - y.GetParameters().Length;
            }
        }

        public T Resolve<T>()
        {
            var abstractionType = typeof(T);

            return (T) Resolve(abstractionType);
        }

        public object Resolve(Type abstractionType)
        {

            if (_factories.ContainsKey(abstractionType))
            {
                return _factories[abstractionType]();
            }
            
            var registered = IsRegistered(abstractionType);

            if (registered)
            {
                var constructors = registered.Value.GetConstructors().ToList();

                constructors.Sort(new ConstructorComparer());

                foreach (var constructor in constructors)
                {
                    var resolved = Resolve(constructor);

                    if (resolved != null)
                    {
                        return resolved;
                    }
                }

                throw new Exception($"Unable to find any resolvable Constructors for {registered.Value.FullName}");
            }


            throw new Exception($"Unable to find any registered implementation for {abstractionType.FullName}");
        }

        private object Resolve(ConstructorInfo constructor)
        {
            var parameters = constructor.GetParameters();

            var types = parameters.Select(p => p.ParameterType).ToArray();

            var values = new object[types.Length];

            for (int i = 0; i < types.Length; i++)
            {
                var type = types[0];

                var resolvedParameter = Resolve(type);

                if (resolvedParameter == null)
                {
                    return null;
                }

                values[i] = resolvedParameter;
            }

            try
            {
                var resolvedService = constructor.Invoke(values);

                if (resolvedService != null)
                {
                    return resolvedService;
                }
            }
            catch (Exception e)
            {
            }

            return null;
        }

        private Result<Type> IsRegistered(Type abstractionType)
        {
            foreach (var type in _registered.Keys)
            {
                if (type == abstractionType)
                {
                    return _registered[type];
                }
            }

            if (!abstractionType.IsInterface && !abstractionType.IsAbstract)
            {
                return abstractionType;
            }

            return null;
        }

        public LiteContainer Register<T>()
        {
            return Register<T, T>();
        }

        public LiteContainer Register<T>(object instance)
        {
            return Register<T>(() => instance);
        }
        
        public LiteContainer Register<T>(Func<object> factory)
        {
            var type = typeof(T);

            if (_factories.ContainsKey(type))
            {
                _factories.Remove(type);
            }
            
            _factories.Add(type,factory);

            return this;
        }

        public LiteContainer Register<TAbstraction, TImplementation>()
            where TImplementation : TAbstraction
        {
            var abstraction = typeof(TAbstraction);

            if (_registered.ContainsKey(abstraction))
            {
                _registered.Remove(abstraction);
            }

            var implementation = typeof(TImplementation);

            _registered.Add(abstraction, implementation);

            return this;
        }

       
    }
}