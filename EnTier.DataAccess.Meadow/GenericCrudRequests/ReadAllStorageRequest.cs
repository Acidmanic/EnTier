using Meadow;
using Meadow.Contracts;

namespace EnTier.DataAccess.Meadow.GenericCrudRequests
{
    public class ReadAllStorageRequest<TStorage> : StorageRequest<MeadowVoid, TStorage>
        where TStorage : class, new()
    {
        protected override string PickProcedureName(NameConvention nameConvention)
        {
            return FullTreeReadWrite()? 
                nameConvention.SelectAllProcedureNameFullTree:
                nameConvention.SelectAllProcedureName;
        }


        public ReadAllStorageRequest() : base(typeof(TStorage))
        {
        }
    }
}