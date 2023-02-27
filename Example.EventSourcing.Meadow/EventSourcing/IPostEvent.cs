using Meadow.Scaffolding.Attributes;

namespace Example.EventSourcing.Meadow.EventSourcing;

[EventStreamPreferences(typeof(long),typeof(long),256,1024)]
public interface IPostEvent
{
    
    public long PostId { get; set; }
    
    public long Timestamp { get; set; }
    
    public string Name { get; set; }
    
}