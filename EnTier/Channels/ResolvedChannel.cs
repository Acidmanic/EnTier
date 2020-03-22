

using System;
using Repository;
using Service;
using Utility;

namespace Channels{




    public class ResolvedChannel<TStorage,TDomain,TId>{


        public string Key{get; private set;}

        public ResolvedChannel(string key){
            Key = key;
        }

        public Func<IService<TDomain,TId>> Service {get; set;}

        public Func<IRepository<TStorage,TId>> Repository {get; set;}

        public Func<IProvider<IUnitOfWork>> UnitOfWorkProvider {get; set;}

    }
}