


using System;
using System.Collections.Generic;
using System.Diagnostics;
using Context;
using Controllers;
using Microsoft.EntityFrameworkCore;
using Utility;

namespace Channels{




    public class ChannelProvider{


        private static readonly Dictionary<Type,Channel> _channels 
                                = new Dictionary<Type, Channel>();

        

        public static void AddChannel<TStorage,TDomain,TTransfer,Tid>
        (EnTierControllerBase<TStorage,TDomain,TTransfer, Tid> controller)
        where TStorage:class
        {

            var key = controller.GetType();

            var configurations = controller.ControllerConfigurations;

            var contextProvider = GetContextProvider(configurations);

            if(!_channels.ContainsKey(key)){

                var channel = CreateChannel<TStorage,TDomain,Tid>(key,contextProvider);

                _channels.Add(key,channel);
            }
        }

        /*
            1) if configured, then return based on that.
            2) search if any Context has implemented
            3) search if there is any DbContext, to make DatabaseContext from
        */
        private static Func<IContext> GetContextProvider(ControllerConfigurations configurations)
        {
            var r = ReflectionService.Make();
            if(configurations.UseConfiguredContextType){
                return r.GetConstructorForType<IContext>(configurations.ContextType)
                .AsDelegate();
            }

            //TODO: Filter only those which contain a IDataset<Storage> property
            var constructor = r.ClearFilters()
                .FilterRemoveImplementers<IEnTierBuiltIn>()
                .FindConstructor<IContext>();

            if(constructor.IsNull){

                //TODO: Filter only those which contain a DbSet<Storage> property
                var dbConstructor = r.FindConstructor<DbContext>(t => r.Extends<DbContext>(t));

                if(!dbConstructor.IsNull){
                    constructor = new Constructor<IContext>(() => new DatabaseContext(
                        dbConstructor.Construct()
                    ));
                }
            }

            return constructor.AsDelegate();
        }

        private static Channel CreateChannel<TStorage, TDomain, Tid>(Type controllerType
            ,Func<IContext> contextProvider)
        where TStorage:class
        {
            
            
            var factory = new BuilderFactory<TStorage,TDomain,Tid>();

            var ret = new Channel(controllerType
                                ,factory.ServiceProvider()
                                ,(obj) => factory.RepositoryBuilder()((IDataset<TStorage>)obj)
                                ,contextProvider
            );
    
            return ret;
        }

        public Channel GetCurrentChannel(){

            var stack = new StackTrace();

            var r = ReflectionService.Make();

            var controllerType = typeof(EnTierControllerBase<,,,>);

            for(int i=0;i<stack.FrameCount;i++){

                var frame = stack.GetFrame(i);

                if(frame.HasMethod()){
                    var method = frame.GetMethod();

                    var type = method.DeclaringType;

                    if(r.IsSpecificOf(type,controllerType)){
                        
                        if( _channels.ContainsKey(type)){
                            return _channels[type];
                        }
                    }                    
                }
            }

            return null;
        }
    }
}