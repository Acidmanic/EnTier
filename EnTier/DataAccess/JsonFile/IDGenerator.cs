using System;
using System.Collections.Generic;

namespace EnTier.DataAccess.JsonFile
{
    public class IDGenerator<TId>
    {
        private readonly List<TId> _takenIds;

        public IDGenerator()
        {
            _takenIds = new List<TId>();
        }

        public IDGenerator(List<TId> takenIds)
        {
            _takenIds = takenIds;
        }

        public TId New()
        {
            var id = PopNewId();

            while (_takenIds.Contains(id))
            {
                id = PopNewId();
            }
            return id;
        }


        private TId PopNewId(TId lastOne = default)
        {
            var idType = typeof(TId);

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
                    return (TId) (object) (Number(lastOne) + 1);
                }
                catch (Exception e)
                {
                    return default;
                }
            }

            if (idType == typeof(string))
            {
                return (TId)(object)Guid.NewGuid().ToString().Replace("-", "");
            }

            if (idType == typeof(Guid))
            {
                return (TId) (object) Guid.NewGuid();
            }

            try
            {
                var constructor = idType.GetConstructor(new Type[] { });

                if (constructor != null)
                {
                    return (TId)constructor.Invoke(new object[]{ });
                }
            }
            catch (Exception)
            {
                // ignored
            }
            throw new Exception($"Given Id Type, {idType.Name}, is not regenerative.");
        }


        private double Number(TId id)
        {
            object idObject = id;

            if (idObject == null)
            {
                return 0;
            }

            return (double) idObject;
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