


using Context;

public interface IDatasetAccessor{

    IDataset<T> Get<T>() where T:class;
}


namespace Channels{



     public class SingletonInjectionDatasetProvider : IDatasetAccessor
    {
        private SingletonInjectionDatasetProvider(){}

        private static SingletonInjectionDatasetProvider instance = null;
        public static SingletonInjectionDatasetProvider Make(){
            var obj = new object();
            lock(obj){
                if(instance == null ){
                    instance = new SingletonInjectionDatasetProvider();
                }
            }
            return instance;
        }


        private object _storedDataset;

        public IDataset<T> Get<T>() where T : class
        {
            return (IDataset<T>) _storedDataset;
        }

        internal void Set(object dataset){
            _storedDataset = dataset;
        }
    }
    public class InjectionDatasetAccessor : IDatasetAccessor
    {
        public IDataset<T> Get<T>() where T : class
        {
            return SingletonInjectionDatasetProvider.Make().Get<T>();
        }
    }

}