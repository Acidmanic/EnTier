



using Repository;

namespace DataAccess
{
    public class DatabaseUnitProvider : IProvider<UnitOfDataAccessBase>
    {
        public UnitOfDataAccessBase Create()
        {
            return new DatabaseUnit(new DataBaseContext());
        }
    }
}