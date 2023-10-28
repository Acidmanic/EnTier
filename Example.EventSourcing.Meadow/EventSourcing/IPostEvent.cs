using Acidmanic.Utilities.Extensions;
using EnTier.Repositories.Attributes;
using Meadow.Scaffolding.Attributes;

namespace Example.EventSourcing.Meadow.EventSourcing;

// You can chose what compression you would use for stored events. you can choose an algorithm or no-compression.
// Default (no attribute) would result to use GZip algorithm for compressing and storing events.
[EventStreamRecordCompression(Compressions.Brotli)]
// Here you MUST specify some information about your stream so Meadow can create your StreamRepository
[EventStreamPreferences(typeof(long),typeof(long),256,1024)]
public interface IPostEvent
{
    
    public long PostId { get; set; }
    
    public long Timestamp { get; set; }
    
    public string Name { get; set; }
    
}