using Meadow;
using Meadow.Reflection.Conventions;

namespace EnTier.DataAccess.Meadow.GenericCrudRequests
{
    public class ReadAllStorageRequest<TStorage> : StorageRequest<MeadowVoid, TStorage>
        where TStorage : class, new()
    {
        protected override string PickProcedureName(NameConvention nameConvention)
        {
            return nameConvention.SelectAllProcedureName;
        }


        public ReadAllStorageRequest() : base(typeof(TStorage))
        {
        }
    }
}