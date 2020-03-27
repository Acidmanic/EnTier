using Context;
using Plugging;
using Repository;
using Service;
using System;
using System.Collections.Generic;
using System.Text;

namespace Components
{
    class ComponentProducer
    {


        public IService<TDomain,Tid> ProduceService<TDomain, Tid>()
        {
            var strategy = new ServiceProducingStrategy();

            var service = strategy.Produce<IService<TDomain, Tid>>();

            return service;
        }

        public IRepository<TStorage,Tid> ProduceRepository<TStorage, Tid>(IDataset<TStorage> dataset)
            where TStorage:class
        {
            var strategy = new RepositoryProducingStrategy();

            var repo = (IRepository < TStorage, Tid> )strategy.Produce<IRepository<TStorage, Tid>>(dataset);

            return repo;
        }

        public IContext ProduceContext()
        {
            var strategy = new ContextProducingStrategy();

            var context = (IContext)strategy.Produce<IContext>();

            return context;
        }

        internal IObjectMapper ProduceMapper()
        {
            var strategy = new MapperProviderStrategy();

            var mapper = (IObjectMapper)strategy.Produce<IObjectMapper>();

            return mapper;
        }
    }
}
