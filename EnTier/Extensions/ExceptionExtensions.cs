using System;

namespace EnTier.Extensions;

public static class ExceptionExtensions
{


    public static Exception InnerMostException(this Exception exception)
    {
        var current = exception;
        
        while (current.InnerException!=null)
        {
            current = current.InnerException;
        }

        return current;
    }
}