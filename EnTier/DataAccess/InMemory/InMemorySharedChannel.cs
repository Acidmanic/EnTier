using System.Collections.Generic;
using Acidmanic.Utilities.Filtering.Models;

namespace EnTier.DataAccess.InMemory
{
    internal static class InMemorySharedChannel
    {

        private static readonly Dictionary<string, object>
            FilterResultsByTypeKey = new Dictionary<string, object>();

        public static List<FilterResult<TId>> FilterResults<TStorage,TId>()
        {

            var key = typeof(TStorage).FullName!.ToLower();

            if (!FilterResultsByTypeKey.ContainsKey(key))
            {
                FilterResultsByTypeKey.Add(key, new List<FilterResult<TId>>());
            }

            return FilterResultsByTypeKey[key] as List<FilterResult<TId>>;

        }
    }
}