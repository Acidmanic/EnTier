



using System;
using Microsoft.Extensions.DependencyInjection;

namespace DIBinding{

    public class MicrosoftDependencyInjectionRegisterer : IDIRegisterer
    {
        private IServiceCollection _registerer;

        public MicrosoftDependencyInjectionRegisterer(IServiceCollection registerer){
            _registerer = registerer;
        }

        public void RegisterFactory<T>(Func<T> factory)
        where T:class
        {
            _registerer.AddTransient<T>(p => factory());
        }

        public void RegisterSingleton<TSrc, TDst>()
        where TSrc: class where TDst: class,TSrc
        {
            _registerer.AddSingleton<TSrc,TDst>();
        }

        public void RegisterTransient<TSrc, TDst>()
        where TSrc: class where TDst: class,TSrc
        {
            _registerer.AddTransient<TSrc,TDst>();
        }

    }
}