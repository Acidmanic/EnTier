using System.Collections.Generic;

namespace EnTier.Models;

public class Chunk<TEntity>
{
    public long TotalCount { get; set; }
    
    public long Size { get; set; }
    
    public long Offset { get; set; }
    
    public IEnumerable<TEntity> Items { get; set; }
}