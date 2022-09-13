using System;
using System.Collections.Generic;
using EnTier.Utility.IdGenerating;

namespace EnTier.Utility
{
    public class IdGenerator<TId>
    {
        private readonly List<TId> _takenIds;

        public IdGenerator()
        {
            _takenIds = new List<TId>();
        }

        public IdGenerator(List<TId> takenIds)
        {
            _takenIds = takenIds;
        }

        public TId New()
        {
            var idType = typeof(TId);

            var id = New(idType);

            return (TId) id;
        }

        public object SetId(object value)
        {
            if (value != null)
            {
                var type = value.GetType();

                var idProperty = type.GetProperty("Id");

                if (idProperty != null)
                {
                    var idType = idProperty.PropertyType;

                    var id = New(idType);

                    idProperty.SetValue(value, id);

                    return id;
                }
            }

            return null;
        }

        public object New(Type idType)
        {
            var id = PopNewId() ;

            while (_takenIds.Contains(id))
            {
                id = PopNewId(id);
            }

            if (id == null)
            {
                return null;
            }

            return id;
            
        }


        private TId PopNewId(TId lastOne = default)
        {
            var incrementor = new IncrementerFactory().Make<TId>();

            if (incrementor != null)
            {
                return incrementor.Increment(lastOne);
            }

            var type = typeof(TId);
            
            if (type == typeof(string))
            {
                return (TId)(object)(Guid.NewGuid().ToString().Replace("-", ""));
            }

            if (type == typeof(Guid))
            {
                return (TId)(object)Guid.NewGuid();
            }
            try
            {
                var constructor = type.GetConstructor(new Type[] { });

                if (constructor != null)
                {
                    return (TId)constructor.Invoke(new object[] { });
                }
            }
            catch (Exception)
            {
                // ignored
            }

            throw new Exception($"Given Id Type, {type.Name}, is not regenerative.");
        }

        private double Number(object id)
        {
            if (id == null)
            {
                return 0;
            }

            return (double) id;
        }

        public void Taken(TId id)
        {
            _takenIds.Add(id);
        }

        public void Free(TId id)
        {
            _takenIds.Remove(id);
        }
    }
}