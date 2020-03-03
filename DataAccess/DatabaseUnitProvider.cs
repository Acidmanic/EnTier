



namespace DataAccess
{
    public class DatabaseUnitProvider : IProvider<DatabaseUnit>
    {
        public DatabaseUnit Create()
        {
            return new DatabaseUnit(new DataBaseContext());
        }
    }
}