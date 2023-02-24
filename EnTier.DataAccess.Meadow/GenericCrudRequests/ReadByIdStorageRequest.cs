
using Meadow.Contracts;

namespace EnTier.DataAccess.Meadow.GenericCrudRequests
{
    public class ReadByIdStorageRequest<TStorage, TId> : ByIdStorageRequest<TStorage,TId,TStorage>
        where TStorage : class, new()
    {
        
        
        protected override string PickProcedureName(NameConvention nameConvention)
        {
            return nameConvention.SelectByIdProcedureName;
        }

    }
}