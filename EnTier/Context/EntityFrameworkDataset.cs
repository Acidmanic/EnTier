


using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace EnTier.Context
{



    internal class EntityFrameworkDataset<T> : IDataset<T>,IEnTierBuiltIn
    where T:class
    {


        private readonly DbSet<T> _dataset;

        public EntityFrameworkDataset(DbSet<T> dataset)
        {
            _dataset = dataset;
        }

        public IQueryable<T> AsQueryable()
        {
            return _dataset;
        }

        public T Remove(T item)
        {
            return _dataset.Remove(item).Entity;
        }

        public T Add(T item){
            return _dataset.Add(item).Entity;
        }
    }
}