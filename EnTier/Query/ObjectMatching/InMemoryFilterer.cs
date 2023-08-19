using System;
using System.Collections.Generic;
using Acidmanic.Utilities.Reflection;
using Acidmanic.Utilities.Reflection.ObjectTree;
using EnTier.Extensions;
using EnTier.Repositories.Models;

namespace EnTier.Query.ObjectMatching
{
    public class InMemoryFilterer<TStorage>
    {
        private readonly AccessNode _idLeaf = TypeIdentity.FindIdentityLeaf<TStorage>();

        public List<FilterResult> PerformFilter(IEnumerable<TStorage> data, FilterQuery filterQuery)
        {
            var filterResults = new List<FilterResult>();
            
            var results = FilterData(data, filterQuery);

            var duration = typeof(TStorage).GetFilterResultExpirationTimeSpan();

            var timestamp = DateTime.Now.Ticks + duration.Ticks;

            var hash = filterQuery.Hash();

            foreach (var result in results)
            {
                filterResults.Add(
                    new FilterResult
                    {
                        Id = hash,
                        ResultId = (long)_idLeaf.Evaluator.Read(result),
                        ExpirationTimeStamp = timestamp
                    });
            }

            return filterResults;
        }


        public List<TStorage> FilterData(IEnumerable<TStorage> data, FilterQuery filterQuery)
        {
            var results = new List<TStorage>();

            foreach (var record in data)
            {
                if (Matches(record, filterQuery))
                {
                    results.Add(record);
                }
            }

            return results;
        }

        public bool Matches(TStorage record, FilterQuery filterQuery)
        {
            var evaluator = new ObjectEvaluator(record);

            var flattenData = evaluator.ToStandardFlatData(o =>
                o.DirectLeavesOnly().IncludeNulls().UseAlternativeTypes());

            var removingFirstSectionLength = (evaluator.RootNode.Name + ".").Length;


            var dataMap = new Dictionary<string, object>();

            foreach (var dataPoint in flattenData)
            {
                var identifier = dataPoint.Identifier
                    .Substring(removingFirstSectionLength, dataPoint.Identifier.Length - removingFirstSectionLength)
                    .ToLower();
                
                dataMap.Add(identifier,dataPoint.Value);
            }
            
            foreach (var filterItem in filterQuery.Items())
            {
                var key = filterItem.Key.ToLower();

                if (!dataMap.ContainsKey(key))
                {
                    return false;
                }

                var value = dataMap[key];

                if (value == null)
                {
                    return false;
                }

                if (value.GetType() != filterItem.ValueType)
                {
                    return false;
                }

                if (!Matches(value, filterItem))
                {
                    return false;
                }
            }

            return true;
        }

        private int CompareAsStrings(object value, string bound)
        {
            var stringValue = value as string;

            return String.Compare(stringValue, bound, StringComparison.Ordinal);
        }
        
        private int CompareAsNumbers(object value, string bound)
        {
            var doubleValue = double.Parse(value.ToString());

            var doubleBound = double.Parse(bound);

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
        
        private int Compare(object value, string bound, Type type)
        {
            if (TypeCheck.IsNumerical(type))
            {
                return CompareAsNumbers(value, bound);
            }

            return CompareAsStrings(value, bound);
        }

        private bool Matches(object value, FilterItem filterQuery)
        {
            switch (filterQuery.EvaluationMethod)
            {
                case EvaluationMethods.SmallerThan:
                    return Compare(value, filterQuery.Maximum, filterQuery.ValueType) <= 0;
                case EvaluationMethods.LargerThan:
                    return Compare(value, filterQuery.Minimum, filterQuery.ValueType) >= 0;
                case EvaluationMethods.BetweenValues:
                    return Compare(value, filterQuery.Maximum, filterQuery.ValueType) <= 0 &&
                           Compare(value, filterQuery.Minimum, filterQuery.ValueType) >= 0;
                case EvaluationMethods.Equal:
                {
                    foreach (var bound in filterQuery.EqualValues)
                    {
                        if (Compare(value, bound, filterQuery.ValueType) == 0)
                        {
                            return true;
                        }
                    }

                    return false;
                }
                    
            }

            return true;
        }
    }
}