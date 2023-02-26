namespace Example.EventSourcing.EntityFramework.EventSourcing;


public interface IPostEvent
{
    
    public long PostId { get; set; }
    
    public long Timestamp { get; set; }
    
    public string Name { get; set; }
    
}