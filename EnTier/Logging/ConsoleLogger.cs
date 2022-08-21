using System;

namespace EnTier.Logging
{
    public class ConsoleLogger : LoggerAdapter
    {
        public ConsoleLogger() : base(Console.WriteLine)
        {
        }
    }
}