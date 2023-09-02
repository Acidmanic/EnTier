using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace EnTier.DataAccess.EntityFramework.FullTreeHandling;

public interface IFullTreeMarker<TStorage> where TStorage : class
{
    
    IQueryable<TStorage> IncludeAsNeeded(DbSet<TStorage> dbSet);
    
}