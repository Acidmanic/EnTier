using EnTier.Binding.Abstractions;
using EnTier.Utility;
using System;
using System.Collections.Generic;

namespace Bootstrap.Starters
{
    internal class DefaultResolver : IDIResolver,IDIRegisterer
    {

        private Dictionary<Type, GenericConstructor> _constructors = new Dictionary<Type, GenericConstructor>();

        private static List<object> _singletons = new List<object>();


        private class GenericConstructor
        {
            private Func<object> _wrapper = () => new object();

            private GenericConstructor() { }


            public static GenericConstructor Build<T>(Constructor<T> wrapee)
            {
                var ret = new GenericConstructor();

                ret._wrapper = () => wrapee.Construct();

                return ret;
            }

            public static GenericConstructor Build<T>(Func<T> wrapee)
            {
                var ret = new GenericConstructor();

                ret._wrapper = () => wrapee;

                return ret;
            }

            public object Eeeeh()
            {
                return _wrapper();
            }
        }

        public void RegisterFactory<TService>(Func<TService> factory) where TService : class
        {
            var type = typeof(TService);

            _constructors.Add(type, GenericConstructor.Build(factory));
        }

        public T Resolve<T>()
        {
            var type = typeof(T);

            var ret = Resolve(type);

            if (ret == null) return default;

            return (T)ret;
        }

        public object Resolve(Type serviceType)
        {
            if (_constructors.ContainsKey(serviceType))
            {
                return _constructors[serviceType].Eeeeh();
            }

            return null;
        }

        void IDIRegisterer.RegisterSingleton<TService, TImplementation>()
        {
            var stype = typeof(TService);

            var itype = typeof(TImplementation);

            var obj = ReflectionService.Make().GetConstructorForType<TImplementation>(itype).Construct();

            var singfac = GenericConstructor.Build<TImplementation>(() => obj);

            _constructors.Add(stype, singfac);
        }

        void IDIRegisterer.RegisterTransient<TService, TImplementation>()
        {
            var stype = typeof(TService);

            var itype = typeof(TImplementation);

            var constructor = ReflectionService.Make().GetConstructorForType<TImplementation>(itype);

            var genericConstructor = GenericConstructor.Build(constructor);

            _constructors.Add(stype, genericConstructor);
        }
    }
}