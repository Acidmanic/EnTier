using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EnTier.Extensions;
using EnTier.Models;
using EnTier.Repositories;
using Microsoft.EntityFrameworkCore;

namespace EnTier.DataAccess.EntityFramework;

public class EntityFrameworkDataBoundRepository<TStorage>:DataBoundRepositoryBase where TStorage : class
{
    public EntityFrameworkDataBoundRepository(DbSet<TStorage> dbSet)
    {
        DbSet = dbSet;
    }

    protected DbSet<TStorage> DbSet { get; }
    
    public override Task<FilterRange> GetDataRangeAsync(string headlessFieldAddress)
    {
        return Task.Run<FilterRange>(() =>
        {
            var lambda = headlessFieldAddress.CreatePropertyPickerLambdaHeadless<TStorage, object>();

            var range = new FilterRange();

            range.Maximum = (DbSet.Select(lambda).Max()).ToString();
            range.Minimum = (DbSet.Select(lambda).Min()).ToString();

            return range;
        });
    }

    public override Task<List<string>> GetExistingValuesAsync(string headlessFieldAddress)
    {
        return Task.Run<List<string>>(() =>
        {
                
            var lambda = headlessFieldAddress.CreatePropertyPickerLambdaHeadless<TStorage, object>();
                
            var existingValues =
                DbSet.Select(lambda).Distinct().ToList()
                    .Select(o => o.ToString()).ToList();

            return existingValues;
        });
    }
}