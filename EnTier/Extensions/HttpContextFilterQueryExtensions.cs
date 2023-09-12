using System;
using Acidmanic.Utilities.Filtering;
using Acidmanic.Utilities.Filtering.Models;
using Microsoft.AspNetCore.Http;

namespace EnTier.Extensions
{
    public static class HttpContextFilterQueryExtensions
    {
        public static FilterQuery GetFilter<TStorageModel>(this HttpContext context, bool fullTree)
        {
            return context.Request.GetFilter<TStorageModel>(fullTree);
        }

        public static FilterQuery GetFilter(this HttpContext context, Type storageModelType, bool fullTree)
        {
            return context.Request.GetFilter(storageModelType, fullTree);
        }

        public static string GetSearchTerms(this HttpContext context)
        {
            return context.Request.GetSearchTerms();
        }

        public static OrderTerm[] GetOrdering(this HttpContext context, Type storageModelType, bool fullTree)
        {
            return context.Request.GetOrdering(storageModelType, fullTree);
        }

        public static OrderTerm[] GetOrdering<TStorageModel>(this HttpContext context, bool fullTree)
        {
            return context.Request.GetOrdering<TStorageModel>(fullTree);
        }
    }
}