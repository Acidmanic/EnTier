namespace ExampleEntityFramework.EventStream;


public interface IPostEvent
{
    
    public long PostId { get; set; }
    
    public long Timestamp { get; set; }
    
    public string Name { get; set; }
    
}