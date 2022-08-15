using Meadow.Reflection.Conventions;
using Meadow.Requests.FieldManipulation;

namespace EnTier.DataAccess.Meadow.GenericCrudRequests
{
    public class UpdateStorageRequest<TStorage> : StorageRequest<TStorage, TStorage>
        where TStorage : class, new()
    {
       
        protected override string PickProcedureName(NameConvention nameConvention)
        {
            return nameConvention.UpdateProcedureName;
        }

        public UpdateStorageRequest() : base(typeof(TStorage))
        {
        }
    }
}