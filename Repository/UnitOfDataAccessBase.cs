




using Microsoft.EntityFrameworkCore;

namespace Repository
{



    public abstract class UnitOfDataAccessBase : IUnitOfWork
    {

        private readonly DbContext _context;


        public UnitOfDataAccessBase(DbContext context)
        {
            _context = context;
        }

        public void Compelete()
        {
            _context.SaveChanges();
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}