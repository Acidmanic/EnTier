using System;
using EnTier.Logging;
using EnTier.Results;
using Microsoft.Extensions.Logging;

namespace EnTier.DependencyInjection
{
    internal class EnTierResolver
    {
        private readonly IResolverFacade _resolverFacade;


        public EnTierResolver(IServiceProvider serviceProvider):this(new DotnetResolverFacade(serviceProvider))
        { }
        
        public EnTierResolver(IResolverFacade resolverFacade)
        {
            this._resolverFacade = resolverFacade;
        }

        public object Resolve(Type type)
        {
            return _resolverFacade.Resolve(type);
        }

        public Result<object> TryResolve(Type type)
        {
            try
            {
                var resolved = _resolverFacade.Resolve(type);

                if (resolved != null)
                {
                    return new Result<object>().Succeed(resolved);
                }
            }
            catch (Exception e)
            {
                EnTierLogging.GetInstance().Logger.LogError(e,"Problem resolving {TypeFullName}",type.FullName);
            }
            return new Result<object>().FailAndDefaultValue();
        }

        public T Resolve<T>()
        {
            var type = typeof(T);

            var resolved = Resolve(type);

            return (T) resolved;
        }

        public Result<T> TryResolve<T>()
        {
            var type = typeof(T);

            var result = TryResolve(type);

            if (result.Success)
            {
                var resolved = result.Value;

                if (resolved is T casted)
                {
                    return new Result<T>().Succeed(casted);
                }
            }
            return new Result<T>().FailAndDefaultValue();
        }
    }
}