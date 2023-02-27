using EnTier.EventSourcing;
using EnTier.EventSourcing.Attributes;
using EnTier.Utility;
using Example.EventSourcing.Meadow.DomainModels;

namespace Example.EventSourcing.Meadow.EventSourcing;

public class PostingAggregate:AggregateBase<Post,IPostEvent,long>
{
    protected override void ManipulateState(IPostEvent @event)
    {
        if (@event is ChangePostTitleEvent titleEvent)
        {
            CurrentState.Title = titleEvent.Title;
            CurrentState.LastModified = titleEvent.Timestamp;
        }

        if (@event is ChangePostContentEvent contentEvent)
        {
            CurrentState.Content = contentEvent.Content;
            CurrentState.LastModified = contentEvent.Timestamp;
        }

        if (@event is CreatePostEvent createPostEvent)
        {
            CurrentState.Content = createPostEvent.Content;
            CurrentState.Title = createPostEvent.Title;
            CurrentState.LastModified = createPostEvent.Timestamp;
            CurrentState.Id = createPostEvent.PostId;
        }
    }

    public void UpdateContent(string content)
    {
        ActionTaken(new ChangePostContentEvent
        {
            Content = content,
            Timestamp = DateTime.Now.Ticks,
            PostId = StreamId
        });
    }


    public void UpdateTitle(string title)
    {
        if (string.IsNullOrWhiteSpace(title))
        {
            throw new PostinException("Title can not be empty.");
        }

        if (title.ToLower().Contains("swearing"))
        {
            throw new PostinException("Title can not contain swearing!!");
        }
        ActionTaken(new ChangePostTitleEvent
        {
            Timestamp = DateTime.Now.Ticks,
            Title = title,
            PostId = StreamId
        });
    }

    [NoStreamIdApi]
    public void CreatePost(string title, string content)
    {
        if (string.IsNullOrWhiteSpace(title))
        {
            throw new PostinException("Title can not be empty.");
        }

        if (title.ToLower().Contains("swearing"))
        {
            throw new PostinException("Title can not contain swearing!!");
        }

        var sid = new UniqueIdGenerator<long>().Generate();
        
        this.Initialize(sid);
        
        ActionTaken(new CreatePostEvent
        {
            Timestamp = DateTime.Now.Ticks,
            Title = title,
            Content = content,
            PostId = sid
        });
        
    }
}