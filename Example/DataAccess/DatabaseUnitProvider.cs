



using Repository;
using Utility;

namespace DataAccess
{
    public class DatabaseUnitProvider : IProvider<IUnitOfWork>
    {
        public IUnitOfWork Create()
        {
            return new DatabaseUnit(new DataBaseContext());
        }
    }
}