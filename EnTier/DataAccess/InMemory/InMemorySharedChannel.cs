using System.Collections.Generic;
using EnTier.Query.Models;
using EnTier.Repositories.Models;

namespace EnTier.DataAccess.InMemory
{
    internal class InMemorySharedChannel
    {
        public static List<FilterResult> FilterResults { get; } = new List<FilterResult>();
    }
}