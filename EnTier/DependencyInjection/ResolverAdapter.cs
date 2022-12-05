using System;

namespace EnTier.DependencyInjection
{
    internal class ResolverAdapter : IResolverFacade
    {
        private readonly Func<Type, object> _resolve;

        public ResolverAdapter(Func<Type, object> resolve)
        {
            _resolve = resolve;
        }

        public object Resolve(Type type)
        {
            return _resolve(type);
        }
    }
}