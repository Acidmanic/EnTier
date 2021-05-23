using System;
using System.Collections.Generic;

namespace EnTier.DataAccess.JsonFile
{
    public class IdGenerator
    {
        private readonly List<object> _takenIds;

        public IdGenerator()
        {
            _takenIds = new List<object>();
        }

        public IdGenerator(List<object> takenIds)
        {
            _takenIds = takenIds;
        }

        public TId New<TId>()
        {
            var idType = typeof(TId);

            var id = New(idType);
            
            return (TId)id;
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
                    
                    idProperty.SetValue(value,id);

                    return id;
                }
            }
            return null;
        }
        
        private object New(Type idType)
        {
            var id = PopNewId(idType);

            while (_takenIds.Contains(id))
            {
                id = PopNewId(idType,id);
            }
            return id;
        }


        private object PopNewId(Type idType, object lastOne = default)
        {
            if (
                idType == typeof(int) || idType == typeof(long) || idType == typeof(short) ||
                idType == typeof(double) || idType == typeof(decimal) || idType == typeof(byte)
                || idType == typeof(float) || idType == typeof(uint) || idType == typeof(ulong) 
                || idType == typeof(ushort) || idType == typeof(sbyte)
                || idType == typeof(Int16) || idType == typeof(Int32) || idType == typeof(Int64) ||
                idType == typeof(Double) || idType == typeof(Decimal) || idType == typeof(Byte)
                || idType == typeof(UInt16) || idType == typeof(UInt32) || idType == typeof(UInt64)
                || idType == typeof(SByte)
                )
            {
                try
                {
                    return (object) (Number(lastOne) + 1);
                }
                catch (Exception e)
                {
                    return default;
                }
            }

            if (idType == typeof(string))
            {
                return Guid.NewGuid().ToString().Replace("-", "");
            }

            if (idType == typeof(Guid))
            {
                return Guid.NewGuid();
            }

            try
            {
                var constructor = idType.GetConstructor(new Type[] { });

                if (constructor != null)
                {
                    return constructor.Invoke(new object[]{ });
                }
            }
            catch (Exception)
            {
                // ignored
            }
            throw new Exception($"Given Id Type, {idType.Name}, is not regenerative.");
        }


        private double Number(object id)
        {
            if (id == null)
            {
                return 0;
            }

            return (double) id;
        }

        public void Taken(object id)
        {
            _takenIds.Add(id);
        }

        public void Free(object id)
        {
            _takenIds.Remove(id);
        }
    }
}