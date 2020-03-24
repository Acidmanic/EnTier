



using System;
using Context;
using Controllers;
using Microsoft.AspNetCore.Mvc;
using Repository;
using Service;

namespace Channels{





    public class Channel{

        private readonly Func<object> _serviceProvider;
        private readonly Func<object,object> _repositoryProvider;

        private readonly Func<IContext> _contextProvider;

        public Type ControllerType {get;private set;}

        public IService<TDomain,Tid> GetService<TDomain,Tid>(){
            return (IService<TDomain,Tid>)_serviceProvider();
        }

        public IRepository<TStorage,Tid> GetRepository<TStorage,Tid>(IDataset<TStorage> dataset)
        where TStorage:class
        {
            return (IRepository<TStorage,Tid>)_repositoryProvider(dataset);
        }

        public IContext CreateContext(){
            return _contextProvider();
        }

        public Channel(Type controllerType
                    ,Func<object> serviceProvider
                    ,Func<object,object> repositoryProvider
                    ,Func<IContext> contextProvider)
        {
            _serviceProvider = serviceProvider;
            _repositoryProvider = repositoryProvider;    
            ControllerType = controllerType;
            _contextProvider = contextProvider;
        }
    }
}