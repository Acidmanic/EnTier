

using Microsoft.EntityFrameworkCore;
using Repository;

namespace Generics{




    public class GenericUnitOfWorkBuilder : IGenericBuilder<IUnitOfWork>
    {
        public object Build<TStorage, TDomain, Tid>() where TStorage : class
        {
            if(EnTierApplication.IsContextBased){

                //TODO: Get From Application
                DbContext context = null;

                return new DatabaseContextGenericUnitOfDataAccess(context);
            }
            else{

                return new NoneContexedGenericUnitOfWork();
            }
        }
    }
}