



using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Utility;

namespace Context{


    public class DatabaseContext : IContext
    {

        private readonly DbContext _context;
        private readonly Dictionary<Type,object> _datasets;

        public DatabaseContext(DbContext context){
            _context = context;

            var r = new CachedReflection().CachePropertiesOf(_context);

            var genericDbSetType = typeof(DbSet<>);
            
            _datasets = new Dictionary<Type, object>();

            r.GetCreatorForTypeWhich<object>(t =>{

                if (r.IsSpecificOf(t.Type,genericDbSetType)){
                    _datasets.Add(t.Type,t.Instanciator());
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
                return (IDataset<T>) _datasets[type];
            }

            //TODO: Null pattern
            return null;
        }
    }
}