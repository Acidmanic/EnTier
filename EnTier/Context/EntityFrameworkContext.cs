



using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using EnTier.Utility;

namespace EnTier.Context
{


    public sealed class EntityFrameworkContext : IContext,IEnTierBuiltIn
    {

        private readonly DbContext _context;
        private readonly Dictionary<Type,object> _datasets;

        public EntityFrameworkContext(DbContext context){
            _context = context;

            var r = new CachedReflection().CachePropertiesOf(_context);

            var genericDbSetType = typeof(DbSet<>);
            
            _datasets = new Dictionary<Type, object>();

            r.GetCreatorForTypeWhich<object>(t =>{

                if (r.IsSpecificOf(t.Type,genericDbSetType)){

                    var keyType = t.Type.GetGenericArguments()[0];

                    _datasets.Add(keyType,t.Instanciator());
                }
                return false;
            });
        }

        public void Apply()
        {
            _context.SaveChanges();
        }

        public void Dispose()
        {
            _context.Dispose();
        }

        public IDataset<T> GetDataset<T>() where T : class
        {
            var type = typeof(T);

            if(_datasets.ContainsKey(type)){
                return new EntityFrameworkDataset<T>((DbSet<T>)_datasets[type]);
            }

            //TODO: Null pattern
            return null;
        }
    }
}