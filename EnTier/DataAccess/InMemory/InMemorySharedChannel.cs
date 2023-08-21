using System.Collections.Generic;
using Acidmanic.Utilities.Filtering.Models;

namespace EnTier.DataAccess.InMemory
{
    internal class InMemorySharedChannel
    {
        public static List<FilterResult> FilterResults { get; } = new List<FilterResult>();
    }
}