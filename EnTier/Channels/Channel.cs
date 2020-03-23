



using System;
using Context;
using Controllers;
using Microsoft.AspNetCore.Mvc;
using Repository;
using Service;

namespace Channels{





    public class Channel{

        private readonly Func<object> _serviceProvider;
        private readonly Func<object> _repositoryProvider;

        public IContext Context {get;private set;}

        public Type ControllerType {get;private set;}

        public IService<TDomain,Tid> GetService<TDomain,Tid>(){
            return (IService<TDomain,Tid>)_serviceProvider();
        }

        public IRepository<TStorage,Tid> GetRepository<TStorage,Tid>(){
            return (IRepository<TStorage,Tid>)_repositoryProvider();
        }

        public Channel(Type controllerType
                    ,Func<object> serviceProvider
                    ,Func<object> repositoryProvider
                    ,IContext context)
        {
            _serviceProvider = serviceProvider;
            _repositoryProvider = repositoryProvider;    
            ControllerType = controllerType;
            Context = context;
        }
    }
}