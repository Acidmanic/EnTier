using System;
using Microsoft.AspNetCore.Routing;

namespace EnTier.DependencyInjection
{
    public class DotnetResolverFacade:IResolverFacade
    {


        private readonly IServiceProvider _serviceProvider;

        public DotnetResolverFacade(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public object Resolve(Type type)
        {
            return _serviceProvider.GetService(type);
        }
    }
}