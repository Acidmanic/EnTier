namespace ExampleEntityFramework.EventSourcing;

public class CreatePostEvent:IPostEvent
{
    public long PostId { get; set; }
    
    public long Timestamp { get; set; } 

    public string Name { get; set; } = "ChangePostTitle";
    
    public string Title { get; set; }
    
    public string Content { get; set; }
}