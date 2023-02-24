namespace EnTier.Repositories.Models;

public class StreamEvent<TEvent, TEventId, TStreamId>
{
    public TEvent Event { get; set; }
        
    public TStreamId StreamId { get; set; }
        
    public TEventId EventId { get; set; }
        
}