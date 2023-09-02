using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace EnTier.DataAccess.EntityFramework.FullTreeHandling;

public class NullFullTreeMarker
{
    
}

public class NullFullTreeMarker<T>:NullFullTreeMarker, IFullTreeMarker<T> where T : class
{
    
    public IQueryable<T> IncludeAsNeeded(DbSet<T> dbSet)
    {
        return dbSet;
    }
}