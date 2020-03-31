using System;
using System.Collections.Generic;
using System.Text;

namespace EnTier.Binding.Abstractions
{
    public interface IDIRegisterer
    {
        void RegisterTransient<TService, TImplementation>() where TService : class where TImplementation : class, TService;

        void RegisterFactory<TService>(Func<TService> factory) where TService : class;

        void RegisterSingleton<TService, TImplementation>() where TService : class where TImplementation : class, TService;
    }
}
