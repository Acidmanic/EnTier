using Acidmanic.Utilities.Reflection.FieldInclusion;
using Meadow.Contracts;

namespace EnTier.DataAccess.Meadow.GenericCrudRequests
{
    public class InsertStorageRequest<TStorage> : StorageRequest<TStorage, TStorage>
        where TStorage : class, new()
    {
        protected override void OnFieldManipulation(IFieldInclusionMarker toStorage, IFieldInclusionMarker fromStorage)
        {
            base.OnFieldManipulation(toStorage, fromStorage);

            if (AutogeneratedFields.Count>0)
            {
                AutogeneratedFields.ForEach(f => toStorage.Exclude(f.Key));
            }
        }


        public InsertStorageRequest() : base(typeof(TStorage))
        {
        }

        protected override string PickProcedureName(NameConvention nameConvention)
        {
            return nameConvention.InsertProcedureName;
        }
    }
}