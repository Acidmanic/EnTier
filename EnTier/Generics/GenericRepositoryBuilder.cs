


using System;
using Microsoft.EntityFrameworkCore;
using Repository;

namespace Generics{




    public class GenericRepositoryBuilder : IGenericBuilder<IRepository>
    {
        public object Build<TStorage, TDomain, Tid>() where TStorage : class
        {
            if(EnTierApplication.IsContextBased){

                DbSet<TStorage> dbSet = null;

                return new GenericDatabaseContextRepository<TStorage,Tid>(dbSet);
            }
            if (!EnTierApplication.IsContextBased){
                //TODO:
                // In Memory Repo or something
            }
            //TODO: NullObject pattern here
            return null;
        }
    }
}