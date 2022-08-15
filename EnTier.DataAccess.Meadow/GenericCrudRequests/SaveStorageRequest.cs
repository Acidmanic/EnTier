using Meadow.Reflection.Conventions;
using Meadow.Requests.FieldManipulation;

namespace EnTier.DataAccess.Meadow.GenericCrudRequests
{
    public class SaveStorageRequest<TStorage> : StorageRequest<TStorage, TStorage>
        where TStorage : class, new()
    {
       
        protected override string PickProcedureName(NameConvention nameConvention)
        {
            return nameConvention.SaveProcedureName;
        }

        public SaveStorageRequest() : base(typeof(TStorage))
        {
        }
    }
}