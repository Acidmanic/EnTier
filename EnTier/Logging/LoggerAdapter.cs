using System;
using Microsoft.Extensions.Logging;

namespace EnTier.Logging
{
    public class LoggerAdapter : ILogger
    {
        private readonly Action<string> _logAction;

        public LoggerAdapter(Action<string> logAction)
        {
            _logAction = logAction;
        }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception,
            Func<TState, Exception, string> formatter)
        {
            var message = formatter(state, exception);

            _logAction(message);
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            return true;
        }

        public IDisposable BeginScope<TState>(TState state)
        {
            return null;
        }
    }
}