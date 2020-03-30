using System;
using System.Collections.Generic;
using System.Text;
using EnTier.Annotations;
using EnTier.Utility;

namespace EnTier.Components
{
    class AutoInjectionComponentProvider : IComponentProvider
    {
        public virtual TInterface Provide<TInterface>(params object[] args)
        {
            var types = ReflectionService.Make()
                    .FilterRemoveImplementers<IEnTierGeneric>()
                    .Filter(t => !t.Type.IsAbstract && !t.Type.IsInterface)
                    .GetTypesWhichImplement(typeof(TInterface));

            if (types.Count > 0)
            {
                var type = types[0];

                var injectedAs = GetInjectingInterface(type);

                if (injectedAs != null)
                    return (TInterface)EnTierApplication.Resolver.Resolve(injectedAs);
            }

            return default;
        }


        // You can have InjectionEntry attribute on Injectable itenrface 
        // Or you can put it on implemented Class with an argument pointing
        // out the injectable interface
        private Type GetInjectingInterface(Type type)
        {
            var ret = ReflectionService.Make().GetInterfaceWithAttribute<InjectionEntry>(type);

            if (ret == null)
            {
                var res = type.GetCustomAttributes(typeof(InjectionEntry), true);

                if (res.Length > 0)
                {
                    ret = ((InjectionEntry)res[0]).InjectionType;
                }
            }

            return ret;
        }
    }
}
