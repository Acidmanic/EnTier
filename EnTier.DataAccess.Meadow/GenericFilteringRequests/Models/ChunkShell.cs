namespace EnTier.DataAccess.Meadow.GenericFilteringRequests.Models
{
    internal class ChunkShell
    {
        public string FilterHash { get; set; }

        public long Offset { get; set; }

        public long Size { get; set; }
    }
}