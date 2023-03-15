using System;

namespace Example.EventSourcing.EntityFramework.EventSourcing;

public class PostinException:Exception
{
    public PostinException(string message) : base(message)
    {
    }
}