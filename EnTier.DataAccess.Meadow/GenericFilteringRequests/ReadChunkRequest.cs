using EnTier.DataAccess.Meadow.GenericFilteringRequests.Models;
using Meadow.Contracts;
using Meadow.Requests;

namespace EnTier.DataAccess.Meadow.GenericFilteringRequests
{
    internal sealed class ReadChunkRequest<TStorage> : MeadowRequest<ChunkShell, TStorage> where TStorage : class, new()
    {
        public ReadChunkRequest(string searchId, long offset, long size) : base(true)
        {
            ToStorage = new ChunkShell
            {
                SearchId = searchId,
                Offset = offset,
                Size = size
            };
        }

        public override string RequestText
        {
            get
            {
                return new NameConvention<TStorage>().ReadChunkProcedureName;
            }
            protected set
            {
                
            }
        }
    }
}