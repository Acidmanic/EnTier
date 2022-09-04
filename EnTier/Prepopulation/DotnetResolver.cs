using System;
using Microsoft.AspNetCore.Builder;

namespace EnTier.Prepopulation
{
    public class DotnetResolver:IServiceResolver
    {

        //private readonly IApplicationBuilder _applicationBuilder;
        private readonly Func<Type,object> _provider;

        public DotnetResolver(IApplicationBuilder applicationBuilder)
        {
            _provider =  t => applicationBuilder.ApplicationServices.GetService(t);
        }

        public DotnetResolver(IServiceProvider serviceProvider)
        {
            _provider = serviceProvider.GetService;
        }

        public object Resolve(Type abstractionType)
        {
            return _provider(abstractionType);
        }
    }
}