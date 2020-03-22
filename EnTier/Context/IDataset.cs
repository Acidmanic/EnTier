


using System.Linq;

namespace Context{



    public interface IDataset<T> where T:class
    {
        
        IQueryable<T> AsQueryable();

        T Remove(T item);

        T Add(T item);

    }
}