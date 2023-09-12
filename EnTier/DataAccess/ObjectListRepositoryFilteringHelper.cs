using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Acidmanic.Utilities.Filtering;
using Acidmanic.Utilities.Filtering.Models;
using Acidmanic.Utilities.Filtering.Utilities;
using Acidmanic.Utilities.Reflection;
using Acidmanic.Utilities.Reflection.ObjectTree;
using EnTier.DataAccess.JsonFile;
using EnTier.Models;

namespace EnTier.DataAccess
{
    internal static class ObjectListRepositoryFilteringHelper
    {
        public static Task RemoveExpiredFilterResultsAsync<TId>(List<FilterResult<TId>> filterResults)
        {
            var now = DateTime.Now.Ticks;

            var expired = filterResults
                .Where(f => f.ExpirationTimeStamp <= now);

            foreach (var filterResult in expired)
            {
                filterResults.Remove(filterResult);
            }

            return Task.CompletedTask;
        }

        public static Task RemoveExpiredFilterResultsAsync<TStorage, TId>(
            List<MarkedFilterResult<TStorage, TId>> filterResults)
        {
            var now = DateTime.Now.Ticks;

            var expired = filterResults
                .Where(f => f.ExpirationTimeStamp <= now);

            foreach (var filterResult in expired)
            {
                filterResults.Remove(filterResult);
            }

            return Task.CompletedTask;
        }


        private static bool IndexHits<TId>(List<SearchIndex<TId>> searchIndex, string[] terms, TId id)
        {
            if (terms == null || terms.Length == 0)
            {
                return true;
            }

            var matchingIndex = searchIndex.FirstOrDefault(s => s.ResultId.Equals(id));

            if (matchingIndex != null)
            {
                return terms.Any(t => matchingIndex.IndexCorpus.Contains(t));
            }

            return false;
        }

        private static bool IndexHits<TStorage, TId>(List<MarkedSearchIndex<TStorage, TId>> searchIndex, string[] terms,
            TId id)
        {
            if (terms == null || terms.Length == 0)
            {
                return true;
            }

            var matchingIndex = searchIndex.FirstOrDefault(s => s.ResultId.Equals(id));

            if (matchingIndex != null)
            {
                return terms.Any(t => matchingIndex.IndexCorpus.Contains(t));
            }

            return false;
        }


        public static Task<IEnumerable<FilterResult<TId>>> PerformFilterIfNeededAsync<TStorage, TId>(
            List<FilterResult<TId>> filterResults,
            List<SearchIndex<TId>> searchIndex,
            AccessNode idLeaf,
            IEnumerable<TStorage> data,
            FilterQuery filterQuery,
            string[] searchTerms = null,
            OrderTerm[] orderTerms = null,
            string searchId = null)
        {
            searchId ??= Guid.NewGuid().ToString("N");
            
            orderTerms ??= new OrderTerm[] { };

            var anyResult = filterResults.Any(f => f.SearchId == searchId);

            if (!anyResult)
            {
                var filteringResults = new ObjectStreamFilterer<TStorage, TId>()
                    .PerformFilter(data, filterQuery,
                        (_, id) => IndexHits(searchIndex, searchTerms, id), orderTerms,searchId);

                filterResults.AddRange(filteringResults);
            }

            return Task.FromResult(filterResults.Where(fr => fr.SearchId == searchId));
        }

        public static Task<IEnumerable<FilterResult<TId>>> PerformFilterIfNeededAsync<TStorage, TId>(
            List<MarkedFilterResult<TStorage, TId>> filterResults,
            List<MarkedSearchIndex<TStorage, TId>> searchIndex,
            AccessNode idLeaf,
            IEnumerable<TStorage> data,
            FilterQuery filterQuery,
            string[] searchTerms = null,
            OrderTerm[] orderTerms = null,
            string searchId = null)
        {
            searchId ??= Guid.NewGuid().ToString("N");

            orderTerms ??= new OrderTerm[] { };

            var anyResult = filterResults.Any(f => f.SearchId == searchId);

            if (!anyResult && idLeaf != null)
            {
                var filteringResults = new ObjectStreamFilterer<TStorage, TId>()
                    .PerformFilter(data, filterQuery,
                        (_, id) => IndexHits(searchIndex, searchTerms, id),
                        orderTerms,searchId);

                filterResults.AddRange(filteringResults.Select(f => f.AsMarked<TStorage, TId>()));
            }

            return Task.FromResult(
                filterResults.Where(fr => fr.SearchId == searchId)
                    .Select(fr => (FilterResult<TId>)fr));
        }

        public static Task<IEnumerable<TStorage>> ReadChunkAsync<TStorage, TId>(
            List<FilterResult<TId>> filterResults,
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

        public static Task<IEnumerable<TStorage>> ReadChunkAsync<TStorage, TId>(
            List<MarkedFilterResult<TStorage, TId>> filterResults,
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

        public static Task<FilterRange> GetFilterRangeAsync<TStorage>(string headlessFieldAddress,
            IEnumerable<TStorage> data)
        {
            var ev = new ObjectEvaluator(typeof(TStorage));

            var fullAddress = ev.RootNode.Name + "." + headlessFieldAddress;

            var node = ev.Map.NodeByAddress(fullAddress);

            var range = new FilterRange();

            foreach (var item in data)
            {
                var fieldValue = node.Evaluator.Read(item);

                fieldValue = NullToDefault(fieldValue, node.Type);

                var smallerThanMin = Compare(fieldValue, range.Minimum, node.Type) < 0;
                var largerThanMax = Compare(fieldValue, range.Maximum, node.Type) > 0;

                if (smallerThanMin || range.Minimum == null)
                {
                    range.Minimum = fieldValue.ToString();
                }

                if (largerThanMax || range.Maximum == null)
                {
                    range.Maximum = fieldValue.ToString();
                }
            }

            return Task.FromResult(range);
        }

        private static object NullToDefault(object fieldValue, Type nodeType)
        {
            if (fieldValue == null)
            {
                if (TypeCheck.IsNumerical(nodeType))
                {
                    return 0;
                }
                else
                {
                    return "";
                }
            }

            return fieldValue;
        }

        public static Task<List<string>> GetExistingValuesAsync<TStorage>(string headlessFieldAddress,
            IEnumerable<TStorage> data)
        {
            var ev = new ObjectEvaluator(typeof(TStorage));

            var fullAddress = ev.RootNode.Name + "." + headlessFieldAddress;

            var node = ev.Map.NodeByAddress(fullAddress);

            var distincts = new List<string>();

            foreach (var item in data)
            {
                var fieldValueString = node.Evaluator.Read(item).ToString();

                if (!distincts.Contains(fieldValueString))
                {
                    distincts.Add(fieldValueString);
                }
            }

            distincts.Sort();

            return Task.FromResult(distincts);
        }

        private static int CompareAsStrings(object value, string bound)
        {
            var stringValue = value as string;

            return String.Compare(stringValue, bound, StringComparison.Ordinal);
        }

        private static int CompareAsNumbers(object value, string bound)
        {
            var doubleValue = double.Parse(value.ToString());

            var doubleBound = string.IsNullOrWhiteSpace(bound) ? 0 : double.Parse(bound);

            var diff = doubleValue - doubleBound;

            var percision = 0.0000001;

            if (diff > percision)
            {
                return 1;
            }

            if (diff < -percision)
            {
                return -1;
            }

            return 0;
        }

        private static int Compare(object value, string bound, Type type)
        {
            if (value == null && bound == null)
            {
                return 0;
            }

            if (value == null && bound != null)
            {
                return -1;
            }

            if (value != null && bound == null)
            {
                return 1;
            }

            if (TypeCheck.IsNumerical(type))
            {
                return CompareAsNumbers(value, bound);
            }

            return CompareAsStrings(value, bound);
        }
    }
}