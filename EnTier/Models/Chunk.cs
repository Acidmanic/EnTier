using System.Collections.Generic;

namespace EnTier.Models;

public class Chunk<TEntity>
{
    public long TotalCount { get; set; }
    
    public long Size { get; set; }
    
    public long Offset { get; set; }
    
    public string SearchId { get; set; }
    
    public IEnumerable<TEntity> Items { get; set; }
    
    public static Chunk<TDst> From<TSrc, TDst>(Chunk<TSrc> chunk,IEnumerable<TDst> items)
    {
        return new Chunk<TDst>
        {
            Items = items,
            Offset = chunk.Offset,
            Size = chunk.Size,
            TotalCount = chunk.TotalCount,
            SearchId = chunk.SearchId
        };
    }
}