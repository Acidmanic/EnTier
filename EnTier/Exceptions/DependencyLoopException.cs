using System;

namespace EnTier.Exceptions
{
    public class DependencyLoopException:Exception
    {
        public DependencyLoopException(string dependencyName) : 
            base($"The key: {dependencyName}, has reached itself as a dependency. This would create a " +
                 $"dependency loop. please re arrange your dependencies correctly.")
        {
        }
    }
}