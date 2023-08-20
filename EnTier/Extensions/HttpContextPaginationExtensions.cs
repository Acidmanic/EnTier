using EnTier.Models;
using Microsoft.AspNetCore.Http;

namespace EnTier.Extensions
{
    public static class HttpContextPaginationExtensions
    {

        public static PaginationQuery GetPagination(this HttpContext context)
        {
            return context.Request.GetPagination();
        }
    }
}