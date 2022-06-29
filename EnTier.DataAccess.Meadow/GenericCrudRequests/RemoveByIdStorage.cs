using Meadow.Reflection.Conventions;

namespace EnTier.DataAccess.Meadow.GenericCrudRequests
{
    public class RemoveByIdStorage<TStorage, TId> : ByIdStorageRequest<TStorage, TId, SuccessShell>
        where TStorage : class, new()
    {
        
        protected override string PickProcedureName(NameConvention nameConvention)
        {
            return nameConvention.DeleteByIdProcedureName;
        }
    }
}