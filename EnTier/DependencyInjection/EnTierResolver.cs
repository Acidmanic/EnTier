using System;
using Acidmanic.Utilities.Results;
using Microsoft.Extensions.Logging;

namespace EnTier.DependencyInjection
{
    internal class EnTierResolver
    {
        private readonly IResolverFacade _resolverFacade;

        public EnTierResolver(IResolverFacade resolverFacade)
        {
            this._resolverFacade = resolverFacade;
        }

        public object Resolve(Type type)
        {
            return _resolverFacade.Resolve(type);
        }

        public Result<object, Exception> TryResolve(Type type)
        {
            Exception exception = null;
            var success = false;
            object value = null;

            try
            {
                var resolved = _resolverFacade.Resolve(type);

                if (resolved != null)
                {
                    success = true;
                    value = resolved;
                }
            }
            catch (Exception e)
            {
                exception = e;
            }

            return new Result<object, Exception>(success, exception, value);
        }

        public T Resolve<T>()
        {
            var type = typeof(T);

            var resolved = Resolve(type);

            return (T)resolved;
        }

        public Result<T, Exception> TryResolve<T>()
        {
            var type = typeof(T);

            var result = TryResolve(type);

            if (result.Success)
            {
                var resolved = result.Primary;

                if (resolved is T casted)
                {
                    return new Result<T, Exception>
                    {
                        Primary = casted,
                        Secondary = null,
                        Success = true
                    };
                }

                return new Result<T, Exception>(false, new Exception("Type Miss Match"), default);
            }
            else
            {
                return new Result<T, Exception>(false, result.Secondary, default);
            }
        }
    }
}