


using System;
using Context;
using DIBinding;
using Repository;
using Service;
using Utility;


public interface IDatasetAccessor{

    IDataset<T> Get<T>() where T:class;
}

namespace Channels{



    /*
        Search For Injectable Type
        Search For Instanciable Type
        Make A Generic One
        Maybe NullObject
    */
    public class BuilderFactory<TStorage,TDomain,TId>
    where TStorage:class
    {
        public Func<IService<TDomain,TId>> ServiceProvider()
        {

            return () => {
                IService<TDomain,TId> ret = null;

                ret = ConstructByAutoInjection<IService<TDomain,TId>>();

                if( ret == null){
                    ret = ConstructByConvention<IService<TDomain,TId>>();
                }

                if(ret == null){
                    ret = new GenericService<TStorage,TDomain,TId>();
                }

                return ret;
            };

        }

        

        public Func<IDataset<TStorage>,IRepository<TStorage,TId>> RepositoryBuilder()
        {

            return (ds) => {
                IRepository<TStorage,TId> ret = null;

                ret = ConstructRepositoryByAutoInjection(ds);
                
                if( ret == null){
                    ret = ConstructByConvention<IRepository<TStorage,TId>>(ds);
                }

                if(ret == null){
                    ret = new GenericRepository<TStorage,TId>(ds);
                }

                return ret;
            };

        }


        private IRepository<TStorage,TId> ConstructRepositoryByAutoInjection
        (IDataset<TStorage> dataset)
        {
            IRepository<TStorage,TId> ret = null;

            var obj = new Object();
            
            lock(obj){
                SingletonInjectionDatasetProvider.Make().Set(dataset);
                ret = ConstructByAutoInjection<IRepository<TStorage,TId>>();
                SingletonInjectionDatasetProvider.Make().Set(null);
            }

            return ret;
        }

        private TInterface ConstructByConvention<TInterface>(params object[] obj)
        where TInterface : class
        {
            var constructor = ReflectionService.Make()
                .FilterRemoveImplementers<IEnTierGeneric>()
                .FindConstructor<TInterface>(obj);

                return constructor.Construct();
        }

        private TInterface ConstructByAutoInjection<TInterface>()
        {
            var types = ReflectionService.Make()
                    .FilterRemoveImplementers<IEnTierGeneric>()
                    .Filter(t => !t.Type.IsAbstract && !t.Type.IsInterface)
                    .GetTypesWhichImplement(typeof(TInterface));

            if(types.Count > 0){
                var type = types[0];

                var injectedAs = GetInjectingInterface(type); 

                if(injectedAs!=null)
                    return (TInterface) EnTierApplication.Resolver.Resolve(injectedAs);
            }

            return default;

        }
        // You can have InjectionEntry attribute on Injectable itenrface 
        // Or you can put it on implemented Class with an argument pointing
        // out the injectable interface
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


    public class SingletonInjectionDatasetProvider : IDatasetAccessor
    {
        private SingletonInjectionDatasetProvider(){}

        private static SingletonInjectionDatasetProvider instance = null;
        public static SingletonInjectionDatasetProvider Make(){
            var obj = new object();
            lock(obj){
                if(instance == null ){
                    instance = new SingletonInjectionDatasetProvider();
                }
            }
            return instance;
        }


        private object _storedDataset;

        public IDataset<T> Get<T>() where T : class
        {
            return (IDataset<T>) _storedDataset;
        }

        internal void Set(object dataset){
            _storedDataset = dataset;
        }
    }


    public class InjectionDatasetAccessor : IDatasetAccessor
    {
        public IDataset<T> Get<T>() where T : class
        {
            return SingletonInjectionDatasetProvider.Make().Get<T>();
        }
    }
}