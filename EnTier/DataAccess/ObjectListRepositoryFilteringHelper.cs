using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Acidmanic.Utilities.Filtering;
using Acidmanic.Utilities.Filtering.Models;
using Acidmanic.Utilities.Filtering.Utilities;
using Acidmanic.Utilities.Reflection;
using Acidmanic.Utilities.Reflection.ObjectTree;
using EnTier.Models;

namespace EnTier.DataAccess
{
    internal static class ObjectListRepositoryFilteringHelper
    {
        public static Task RemoveExpiredFilterResultsAsync(List<FilterResult> filterResults)
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
        
        public static Task<FilterRange> GetFilterRangeAsync<TStorage>(string headlessFieldAddress,IEnumerable<TStorage> data)
        {
            var ev = new ObjectEvaluator(typeof(TStorage));

            var fullAddress = ev.RootNode.Name + "." + headlessFieldAddress;

            var node = ev.Map.NodeByAddress(fullAddress);

            var range = new FilterRange();

            foreach (var item in data)
            {
                var fieldValue = node.Evaluator.Read(item);

                fieldValue = NullToDefault(fieldValue,node.Type);

                var smallerThanMin = Compare(fieldValue, range.Minimum,node.Type) < 0;
                var largerThanMax = Compare(fieldValue, range.Maximum,node.Type) > 0;

                if (smallerThanMin || range.Minimum ==null)
                {
                    range.Minimum = fieldValue.ToString();
                }

                if (largerThanMax || range.Maximum ==null)
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

        public static Task<List<string>> GetExistingValuesAsync<TStorage>(string headlessFieldAddress,IEnumerable<TStorage> data)
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

            var doubleBound = string.IsNullOrWhiteSpace(bound)? 0 : double.Parse(bound);

            var diff =  doubleValue - doubleBound;

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