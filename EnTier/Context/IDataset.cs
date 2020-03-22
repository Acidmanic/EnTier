


using System.Linq;

namespace Context{



    public interface IDataset<T> where T:class
    {
        
        IQueryable<T> AsQueryable();

        void Remove(T item);

    }
}