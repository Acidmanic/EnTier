using Acidmanic.Utilities.Reflection.Dynamics;
using Litbid.DataAccess.Meadow.EnTier.DataAccess.Meadow;
using Meadow.Reflection.Conventions;

namespace EnTier.DataAccess.Meadow.GenericCrudRequests
{
    public abstract class ByIdStorageRequest<TStorage, TId, TOut> : StorageRequest<object, TOut>
        where TStorage : class, new() where TOut : class, new()
    {
        public TId Id { get; set; }


        protected ByIdStorageRequest() : base(typeof(TStorage))
        {
        }

        protected override string PickProcedureName(NameConvention nameConvention)
        {
            return nameConvention.SelectByIdProcedureName;
        }


        public override object ToStorage
        {
            get
            {
                var idShell = new ModelBuilder("IdShell")
                    .AddProperty(IdField.Name, IdField.Type).BuildObject();

                PropertyWrapper.Create(IdField.Name, IdField.Type, idShell).Value = Id;

                return idShell;
            }
            set
            {
                //ignore
            }
        }
    }
}