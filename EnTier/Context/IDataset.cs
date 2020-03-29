


using System.Linq;

namespace EnTier.Context
{



    public interface IDataset<T> where T:class
    {
        
        IQueryable<T> AsQueryable();

        T Remove(T item);

        T Add(T item);

    }
}