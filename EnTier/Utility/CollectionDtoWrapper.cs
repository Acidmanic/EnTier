using System;
using System.Collections.Generic;
using System.Linq;
using Acidmanic.Utilities.Reflection.Dynamics;
using Acidmanic.Utilities.Reflection.ObjectTree;
using EnTier.Extensions;

namespace EnTier.Utility
{
    public class CollectionDtoWrapper<TModel>
    {
        private readonly Action<IEnumerable<TModel>,object> _setter;
        private readonly Func<Object> _instantiator;
        public CollectionDtoWrapper():this(typeof(TModel).Name.Plural())
        {
        }
        
        public CollectionDtoWrapper(string name)
        {
            var type = typeof(TModel);
            
            var builder = new ModelBuilder("ResponseWrap").AddProperty(name, typeof(List<TModel>));

            var wrappedType = builder.Build();

            var property = wrappedType.GetProperties()
                .FirstOrDefault(p => p.Name == name);
            
            _instantiator = () => builder.BuildObject();

            
            _setter = (data, obj) =>
            {
                var listedData = new List<TModel>();
            
                listedData.AddRange(data);
                
               property?.SetValue(obj,listedData);
            };
               
        }

        public object Wrap(IEnumerable<TModel> data)
        {
            var wrapped = _instantiator();

            _setter(data, wrapped);

            return wrapped;
        }
    }
}