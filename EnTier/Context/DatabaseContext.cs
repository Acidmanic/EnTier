



using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Utility;

namespace Context{


    public class DatabaseContext : IContext
    {

        private readonly DbContext _context;
        private readonly Dictionary<Type,object> _datasets;

        private class DbSet:DbSet<object>{}

        public DatabaseContext(DbContext context){
            _context = context;

            var r = new CachedReflection().CachePropertiesOf(_context);

            var genericDbSetType = typeof(DbSet).GetGenericTypeDefinition();

            var props = r.FindConstructors<object>(t => r.Extends(t,genericDbSetType) );

            _datasets = new Dictionary<Type, object>();

            foreach(var p in props){
                var dbset = p.Construct();

                var gType = dbset.GetType().GenericTypeArguments[0];

                _datasets.Add(gType,dbset);
            }
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