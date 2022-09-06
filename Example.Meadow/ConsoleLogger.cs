using System;
using Microsoft.Extensions.Logging;

namespace Example.Meadow
{
    public class ConsoleLogger:ILogger
    {
        public IDisposable BeginScope<TState>(TState state)
        {
            return null;
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            return true;
        }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            var message = logLevel.ToString() + ": " + formatter(state, exception);

            Console.WriteLine(message);
        }
    }
}