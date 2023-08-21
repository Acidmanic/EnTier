using System;
using EnTier.Filtering;
using Microsoft.AspNetCore.Http;

namespace EnTier.Extensions
{
    public static  class HttpContextFilterQueryExtensions
    {

        public static FilterQuery GetFilter<TStorageModel>(this HttpContext context)
        {
            return context.Request.GetFilter<TStorageModel>();
        }
        
        public static FilterQuery GetFilter(this HttpContext context,Type storageModelType)
        {
            return context.Request.GetFilter(storageModelType);
        }
    }
}