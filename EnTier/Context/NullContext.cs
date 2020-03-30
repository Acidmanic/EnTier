



namespace EnTier.Context
{



    internal class NullContext : IContext,IEnTierBuiltIn
    {
        public void Apply()
        {

        }

        public void Dispose()
        {
        }

        public IDataset<T> GetDataset<T>() where T : class
        {
            return new NullDataset<T>();
        }
    }
}