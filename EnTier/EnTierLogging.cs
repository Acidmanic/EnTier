using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace EnTier
{
    internal class EnTierLogging
    {
        private static EnTierLogging _instance = null;
        private static object _locker = new object();

        private EnTierLogging()
        {
            
        }

        public static EnTierLogging GetInstance()
        {
            lock (_locker)
            {
                if (_instance == null)
                {
                    _instance= new EnTierLogging();
                }
            }

            return _instance;
        }
        
        
        public ILogger Logger { get; private set; } = NullLogger.Instance;


        public void Set(ILogger logger)
        {
            Logger = logger;
        }

    }
}