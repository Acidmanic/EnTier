



using System;

namespace DIBinding{



    public interface IDIResolver
    {

        T Resolve<T>();
        
    }


    public interface IDIRegisterer
    {
        void RegisterTransient<TService,TImplementation>() where TService: class where TImplementation: class,TService;

        void RegisterFactory<TService>(Func<TService> factory) where TService:class;

        void RegisterSingleton<TService,TImplementation>() where TService: class where TImplementation: class,TService;
    }
}