using System.Linq;
using Acidmanic.Utilities.Results;
using Microsoft.AspNetCore.Http;

namespace EnTier.Extensions
{
    public static class QueryCollectionExtensions
    {

        public static Result<int> ReadIntByKey(this IQueryCollection queries, string key)
        {
            var foundKey = queries.FindKey(key);

            if (foundKey)
            {
                var stringValue = queries[foundKey.Value].FirstOrDefault();

                if (int.TryParse(stringValue, out var value))
                {
                    return new Result<int>(true,value);
                }
            }

            return new Result<int>().FailAndDefaultValue();
        }
        
        public static Result<string> FindKey(this IQueryCollection queries, string keyToFind)
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