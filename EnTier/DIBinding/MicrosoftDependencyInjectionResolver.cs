



using System;
using Microsoft.Extensions.DependencyInjection;

namespace DIBinding{

    public class MicrosoftDependencyInjectionResolver : IDIResolver
    {
        private IServiceProvider _resolver;

        public MicrosoftDependencyInjectionResolver(IServiceProvider resolver){
            _resolver = resolver;
        }
        public T Resolve<T>()
        {
            return _resolver.GetService<T>();
        }
    }
}