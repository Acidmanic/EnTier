namespace ExampleEntityFramework.EventSourcing;

public class ChangePostContentEvent:IPostEvent
{
    public long PostId { get; set; }
    
    public long Timestamp { get; set; } 

    public string Name { get; set; } = "ChangePostContent";
    
    public string Content { get; set; }
}