


using System;
using Repository;
using Service;
using Utility;

namespace Channels{




    public class TypedChannel<TStorage,TDomain,TId>{


        public Type ServiceGenericType(){
            return typeof(IService<TDomain,TId>);
        }

        public Type UnitOfWorkProviderType(){
            return typeof(IProvider<IUnitOfWork>);
        }

        public Type UnitOfWorkType(){
            return typeof(IUnitOfWork);
        }

        public Type RepositoryType(){
            return typeof(IRepository<IUnitOfWork>);
        }
    }
}