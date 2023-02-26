using System;

namespace ExampleEntityFramework.EventSourcing;

public class PostinException:Exception
{
    public PostinException(string message) : base(message)
    {
    }
}