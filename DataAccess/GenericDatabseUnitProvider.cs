



namespace  DataAccess
{
    public class GenericDatabaseUnitProvider : IProvider<GenericDatabaseUnit>
    {
        public GenericDatabaseUnit Create()
        {
            return new GenericDatabaseUnit(new DataBaseContext());
        }
    }
}