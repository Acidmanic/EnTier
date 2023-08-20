using System.Linq;
using System.Xml;
using EnTier.Models;
using Microsoft.AspNetCore.Http;

namespace EnTier.Extensions
{
    public static class HttpRequestPaginationExtensions
    {


        public static PaginationQuery GetPagination(this HttpRequest request)
        {
            var queries = request.Query;

            var foundIndex = queries.ReadIntByKey("pageIndex");

            if (foundIndex)
            {
                var foundSize = queries.ReadIntByKey("pageSize");

                if (foundSize)
                {
                    return new PaginationQuery
                    {
                        PageIndex = foundIndex.Value,
                        PageSize = foundSize.Value
                    };
                }
            }

            return new PaginationQuery
            {
                PageIndex = 1,
                PageSize = int.MaxValue
            };
        }
    }
}