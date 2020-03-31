



using System;
using EnTier.Binding.Abstractions;
using Microsoft.Extensions.DependencyInjection;

namespace EnTier.Binding{

    internal class MicrosoftDependencyInjectionResolver : IDIResolver
    {
        private readonly IServiceProvider _resolver;

        public MicrosoftDependencyInjectionResolver(IServiceProvider resolver){
            _resolver = resolver;
        }
        public T Resolve<T>()
        {
            return _resolver.GetService<T>();
        }

        public object Resolve(Type serviceType){
            return _resolver.GetService(serviceType);
        }
    }
}