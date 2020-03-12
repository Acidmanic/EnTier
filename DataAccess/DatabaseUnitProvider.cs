



using Repository;
using Utility;

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