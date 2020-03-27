using Context;
using System;
using System.Collections.Generic;
using System.Text;
using Utility;

namespace Components
{
    class AppConfigurationContextProvider : IComponentProvider
    {
        public T Provide<T>(params object[] args)
        {
            if(typeof(T) == typeof(IContext))
            {
                if(EnTierApplication.UseConfiguredContextType)
                {
                    var contextType = EnTierApplication.ContextType;

                    var constructor = ReflectionService.Make().GetConstructorForType<IContext>(contextType);

                    return (T)constructor.Construct();
                }
            }
            return default;
        }
    }
}
