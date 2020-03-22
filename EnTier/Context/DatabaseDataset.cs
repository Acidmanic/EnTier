


using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace Context{



    public class DatabaseDataset<T> : IDataset<T>
    where T:class
    {


        private readonly DbSet<T> _dataset;

        public DatabaseDataset(DbSet<T> dataset)
        {
            _dataset = dataset;
        }

        public IQueryable<T> AsQueryable()
        {
            return _dataset;
        }

        public void Remove(T item)
        {
            _dataset.Remove(item);
        }

    }
}