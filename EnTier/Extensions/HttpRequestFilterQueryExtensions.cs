using System;
using System.Linq;
using Acidmanic.Utilities.Reflection.ObjectTree;
using Acidmanic.Utilities.Results;
using EnTier.Query;
using EnTier.Query.Attributes;
using Microsoft.AspNetCore.Http;

namespace EnTier.Extensions
{
    public static class HttpRequestFilterQueryExtensions
    {
        public static FilterQuery GetFilter<TStorageModel>(this HttpRequest request)
        {
            var storageType = typeof(TStorageModel);

            return GetFilter(request, storageType);
        }

        public static FilterQuery GetFilter(this HttpRequest request, Type storageModelType)
        {
            var evaluator = new ObjectEvaluator(storageModelType);

            var leaves = evaluator.RootNode.GetDirectLeaves()
                .Where(l => IsFilterField(l));

            var query = new FilterQuery();

            var requestQueries = request.Query;

            foreach (var leaf in leaves)
            {
                var foundKey = FindKey(requestQueries, leaf.Name);

                if (foundKey)
                {
                    var item = new FilterItem();

                    var values = requestQueries[foundKey.Value];

                    foreach (var value in values)
                    {
                        ApplyQueryItemOnFilterItem(item, value);
                    }

                    item.Key = leaf.Name;
                    item.ValueType = leaf.Type;

                    query.Add(item);
                }
            }

            return query;
        }

        private static bool IsFilterField(AccessNode leaf)
        {
            foreach (var attribute in leaf.PropertyAttributes)
            {
                if (attribute is FilterFieldAttribute)
                {
                    return true;
                }
            }

            return false;
        }

        private static void ApplyQueryItemOnFilterItem(FilterItem item, string queryValue)
        {
            if (queryValue.StartsWith("<"))
            {
                item.Maximum = queryValue.Substring(1, queryValue.Length - 1);
                item.EvaluationMethod = EvaluationMethods.SmallerThan;
                return;
            }

            if (queryValue.StartsWith(">"))
            {
                item.Minimum = queryValue.Substring(1, queryValue.Length - 1);
                item.EvaluationMethod = EvaluationMethods.LargerThan;
                return;
            }

            if (queryValue.Contains("<>", StringComparison.Ordinal))
            {
                var segments = queryValue.Split("<>", StringSplitOptions.RemoveEmptyEntries);

                if (segments.Length > 1)
                {
                    item.Minimum = segments[0];
                    item.Maximum = segments[1];
                    item.EvaluationMethod = EvaluationMethods.BetweenValues;
                    return;
                }
            }

            item.EqualValues.Add(queryValue);
            item.EvaluationMethod = EvaluationMethods.Equal;
        }


        private static Result<string> FindKey(IQueryCollection queries, string keyToFind)
        {
            keyToFind = keyToFind.ToLower();

            foreach (var key in queries.Keys)
            {
                if (key.ToLower() == keyToFind)
                {
                    return new Result<string>(true, key);
                }
            }

            return new Result<string>().FailAndDefaultValue();
        }
    }
}