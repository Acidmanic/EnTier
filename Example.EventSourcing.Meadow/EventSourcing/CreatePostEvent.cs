namespace Example.EventSourcing.Meadow.EventSourcing;

public class CreatePostEvent:IPostEvent
{
    public long PostId { get; set; }
    
    public long Timestamp { get; set; } 

    public string Name { get; set; } = "Create Post";
    
    public string Title { get; set; }
    
    public string Content { get; set; }
}