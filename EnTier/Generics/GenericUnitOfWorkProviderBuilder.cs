


using Providers;
using Repository;
using Utility;

namespace Generics{




    public class GenericUnitOfWorkProviderBuilder : IGenericBuilder<IProvider<IUnitOfWork>>{
        
        public object Build<TStorage, TDomain, Tid>() where TStorage : class
        {
            if(EnTierApplication.IsContextBased){

                return new DatabaseContextUnitofWorkProvider<TStorage>();
            }else{
                return new NoneContexedGenericUnitOfWorkProvider();
            }
        }
    }
}