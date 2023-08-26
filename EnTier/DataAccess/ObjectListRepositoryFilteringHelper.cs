using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Acidmanic.Utilities.Filtering;
using Acidmanic.Utilities.Filtering.Models;
using Acidmanic.Utilities.Filtering.Utilities;
using Acidmanic.Utilities.Reflection;
using Acidmanic.Utilities.Reflection.ObjectTree;

namespace EnTier.DataAccess
{
    internal static class ObjectListRepositoryFilteringHelper
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

        public static Task<IEnumerable<FilterResult>> PerformFilterIfNeededAsync<TStorage>(
            List<FilterResult> filterResults,
            AccessNode idLeaf,
            IEnumerable<TStorage> data,
            FilterQuery filterQuery,
            string searchId = null)
        {
            searchId ??= Guid.NewGuid().ToString("N");

            var anyResult = filterResults.Any(f => f.SearchId == searchId);

            if (!anyResult && idLeaf != null && TypeCheck.IsNumerical(idLeaf.Type))
            {
                var filteringResults = new ObjectStreamFilterer<TStorage>()
                    .PerformFilter(data, filterQuery,searchId);

                filterResults.AddRange(filteringResults);
            }

            return Task.FromResult(filterResults as IEnumerable<FilterResult>);
        }

        public static Task<IEnumerable<TStorage>> ReadChunkAsync<TStorage>(
            List<FilterResult> filterResults,
            AccessNode idLeaf,
            IEnumerable<TStorage> data,
            int offset, int size, string searchId)
        {
            var values = new List<TStorage>();

            var skipped = 0;

            foreach (var result in filterResults)
            {
                if (result.SearchId == searchId)
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