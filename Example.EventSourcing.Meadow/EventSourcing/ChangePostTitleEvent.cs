namespace Example.EventSourcing.Meadow.EventSourcing;

public class ChangePostTitleEvent:IPostEvent
{
    public long PostId { get; set; }
    
    public long Timestamp { get; set; } 

    public string Name { get; set; } = "ChangePostTitle";
    
    public string Title { get; set; }
}