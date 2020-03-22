


using System;
using Context;
using DIBinding;
using Repository;
using Service;
using Utility;

namespace Channels{



    /*
        Search For Injectable Type
        Search For Instanciable Type
        Make A Generic One
        Maybe NullObject
    */
    public class BuilderFactory<TStorage,TDomain,TId>where TStorage:class{


        public Func<TInterface> MakeBuilder<TInterface>(params object[] args)
        where TInterface:class 
        {

            return () => {
                TInterface ret = null;

                ret = ConstructByInjection<TInterface>();

                if( ret == null){
                    ret = ConstructByInstanciation<TInterface>(args);
                }

                if(ret == null){
                    ret = BuildGenericItem<TInterface>(args);
                }

                return ret;
            };

        }


        private TInterface BuildGenericItem<TInterface>(params object[] args)
        {
            var type = typeof(TInterface);

            var r = ReflectionService.Make();

            if (r.Implements<IService>(type)){
                return (TInterface)(object)
                    new GenericService<TStorage,TDomain,TDomain>();
            }
            if (r.Implements<IRepository>(type)){
                return (TInterface)(object)
                    new GenericRepository<TStorage,TId>
                    ((IDataset<TStorage>)args[0]);
            }

            return default;

        }

        private TInterface ConstructByInstanciation<TInterface>(object[] args) where TInterface : class
        {
            var constructor = ReflectionService.Make()
                .FilterRemoveImplementers<IEnTierGeneric>()
                .FindConstructor<TInterface>(args);

                return constructor.Construct();
        }

        private TInterface ConstructByInjection<TInterface>()
        {
            var types = ReflectionService.Make()
                    .FilterRemoveImplementers<IEnTierGeneric>()
                    .GetTypesWhichImplement(typeof(TInterface));

            if(types.Count > 0){
                var type = types[0];

                var injectedAs = GetInjectingInterface(type); 

                if(injectedAs!=null)
                    return (TInterface) EnTierApplication.Resolver.Resolve(injectedAs);
            }

            return default;

        }

        private Type GetInjectingInterface(Type type)
        {
            var ret = ReflectionService.Make().GetInterfaceWithAttribute<InjectionEntry>(type);

            if (ret == null){
                var res = type.GetCustomAttributes(typeof(InjectionEntry),true);

                if(res.Length>0){
                    ret = ((InjectionEntry)res[0]).InjectionType;
                }
            }

            return ret;
        }
    }
}