using System;
using System.Collections.Generic;
using System.Linq;
using Acidmanic.Utilities.Filtering;
using Acidmanic.Utilities.Filtering.Attributes;
using Acidmanic.Utilities.Filtering.Extensions;
using Acidmanic.Utilities.Filtering.Models;
using Acidmanic.Utilities.Filtering.Utilities;
using Acidmanic.Utilities.Reflection.ObjectTree;
using Microsoft.AspNetCore.Http;

namespace EnTier.Extensions
{
    public static class HttpRequestFilterQueryExtensions
    {
        public static FilterQuery GetFilter<TStorageModel>(this HttpRequest request, bool fullTree)
        {
            var storageType = typeof(TStorageModel);

            return GetFilter(request, storageType, fullTree);
        }

        public static FilterQuery GetFilter(this HttpRequest request, Type storageModelType, bool fullTree)
        {
            var evaluator = new ObjectEvaluator(storageModelType);

            var leaves = evaluator.Map.Nodes
                .Where(n => n.IsLeaf)
                .Where(n => n.Depth == 1 || fullTree)
                .Where(IsFilterField);

            var query = new FilterQuery();

            query.EntityType = storageModelType;

            var requestQueries = request.Query;

            //var requestFormFields = request.Form;

            //var jsonBody = request...readJsonDictionary

            foreach (var leaf in leaves)
            {
                var key = evaluator.Map.FieldKeyByNode(leaf).Headless().ToString();

                var foundKey = requestQueries.FindKey(key);

                if (foundKey)
                {
                    var item = new FilterItem();

                    var values = requestQueries[foundKey.Value];

                    foreach (var value in values)
                    {
                        ApplyQueryItemOnFilterItem(item, value);
                    }

                    item.Key = key;
                    item.ValueType = leaf.Type;

                    query.Add(item);
                }
            }

            return query;
        }


        public static OrderTerm[] GetOrdering<TStorageModel>(this HttpRequest request, bool fullTree)
        {
            var storageType = typeof(TStorageModel);

            return GetOrdering(request, storageType, fullTree);
        }

        public static OrderTerm[] GetOrdering(this HttpRequest request, Type storageModelType, bool fullTree)
        {
            var evaluator = new ObjectEvaluator(storageModelType);

            var leaves = evaluator.Map.Nodes
                .Where(n => n.IsLeaf)
                .Where(n => n.Depth == 1 || fullTree);
            //.Where(IsFilterField);

            var orderingTermKeysByFieldKey = new Dictionary<string,string>();

            foreach (var leaf in leaves)
            {
                var key = evaluator.Map.FieldKeyByNode(leaf).Headless().ToString().ToLower();
                var orderTermKey = fullTree ? evaluator.Map.FieldKeyByNode(leaf).Headless().ToString() : leaf.Name;
                
                orderingTermKeysByFieldKey.Add(key,orderTermKey);
            }

            var requestQueries = request.Query;

            //o=+Surname>-Age>+Job.IncomeInRials

            var orders = new List<OrderTerm>();

            if (requestQueries.ContainsKey("o"))
            {
                var ordersExpressions = requestQueries["o"];

                foreach (var value in ordersExpressions)
                {
                    var stringTerms = value.Split('>', StringSplitOptions.RemoveEmptyEntries);

                    foreach (var stringTerm in stringTerms)
                    {
                        var asc = stringTerm.StartsWith('+');
                        var des = stringTerm.StartsWith('-');
                        if (asc || des)
                        {
                            var key = stringTerm.Substring(1, stringTerm.Length - 1).ToLower();

                            if (orderingTermKeysByFieldKey.ContainsKey(key))
                            {
                                orders.Add(new OrderTerm
                                {
                                    Key = orderingTermKeysByFieldKey[key],
                                    Sort = asc ? OrderSort.Ascending : OrderSort.Descending
                                });
                            }
                        }
                    }
                }
            }

            return orders.ToArray();
        }

        public static string GetSearchTerms(this HttpRequest request)
        {
            var requestQueries = request.Query;

            var foundKey = requestQueries.FindKey("q");

            if (foundKey)
            {
                return string.Join('+', requestQueries[foundKey.Value].ToArray());
            }

            return null;
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
                item.ValueComparison = ValueComparison.SmallerThan;
                return;
            }

            if (queryValue.StartsWith(">"))
            {
                item.Minimum = queryValue.Substring(1, queryValue.Length - 1);
                item.ValueComparison = ValueComparison.LargerThan;
                return;
            }

            if (queryValue.Contains("<>", StringComparison.Ordinal))
            {
                var segments = queryValue.Split("<>", StringSplitOptions.RemoveEmptyEntries);

                if (segments.Length > 1)
                {
                    item.Minimum = segments[0];
                    item.Maximum = segments[1];
                    item.ValueComparison = ValueComparison.BetweenValues;
                    return;
                }
            }
            
            if (queryValue.StartsWith("!", StringComparison.Ordinal))
            {
                var equalityValue = queryValue.Substring(1, queryValue.Length - 1);
                item.EqualityValues.Add(equalityValue);
                item.ValueComparison = ValueComparison.NotEqual;
                return;
            }

            item.EqualityValues.Add(queryValue);
            item.ValueComparison = ValueComparison.Equal;
        }
    }
}