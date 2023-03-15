using System;

namespace Example.EventSourcing.Meadow.EventSourcing;

public class PostinException:Exception
{
    public PostinException(string message) : base(message)
    {
    }
}