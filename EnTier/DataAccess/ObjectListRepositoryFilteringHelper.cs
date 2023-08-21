using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Acidmanic.Utilities.Reflection;
using Acidmanic.Utilities.Reflection.ObjectTree;
using EnTier.DataAccess.InMemory;
using EnTier.Query;
using EnTier.Query.Models;
using EnTier.Query.ObjectMatching;
using EnTier.Repositories.Models;

namespace EnTier.DataAccess
{
    internal class ObjectListRepositoryFilteringHelper
    {
        public static Task RemoveExpiredFilterResultsAsync(List<FilterResult> filterResults)
        {
            var now = DateTime.Now.Ticks;

            var expireds = filterResults
                .Where(f => f.ExpirationTimeStamp <= now);

            foreach (var filterResult in expireds)
            {
                filterResults.Remove(filterResult);
            }

            return Task.CompletedTask;
        }

        public static Task PerformFilterIfNeededAsync<TStorage>(
            List<FilterResult> filterResults,
            AccessNode idLeaf,
            IEnumerable<TStorage> data,
            FilterQuery filterQuery)
        {
            var hash = filterQuery.Hash();

            var anyResult = filterResults.Any(f => f.FilterHash == hash);

            if (!anyResult && idLeaf != null && TypeCheck.IsNumerical(idLeaf.Type))
            {
                var filteringResults = new ObjectStreamFilterer<TStorage>()
                    .PerformFilter(data, filterQuery);

                filterResults.AddRange(filteringResults);
            }

            return Task.CompletedTask;
        }

        public static Task<IEnumerable<TStorage>> ReadChunkAsync<TStorage>(
            List<FilterResult> filterResults,
            AccessNode idLeaf,
            IEnumerable<TStorage> data,
            int offset, int size, string hash)
        {
            var values = new List<TStorage>();

            var skipped = 0;

            foreach (var result in filterResults)
            {
                if (result.FilterHash == hash)
                {
                    if (skipped < offset)
                    {
                        skipped++;
                    }
                    else
                    {
                        if (values.Count >= size)
                        {
                            break;
                        }

                        var value = data.FirstOrDefault(d => idLeaf.Evaluator.Read(d).Equals(result.ResultId));

                        values.Add(value);
                    }
                }
            }

            return Task.FromResult((IEnumerable<TStorage>)values);
        }
    }
}