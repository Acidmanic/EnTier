


using System;
using System.Collections.Generic;
using System.Diagnostics;
using Controllers;
using Utility;

namespace Channels{




    public class ChannelProvider{


        private static readonly Dictionary<Type,Channel> _channels 
                                = new Dictionary<Type, Channel>();

        

        public static void AddChannel<TController,TStorage,TDomain,Tid>(){

            var key = typeof(TController);

            if(!_channels.ContainsKey(key)){

                var channel = CreateChannel<TStorage,TDomain,Tid>();

                _channels.Add(key,channel);
            }
        }

        private static Channel CreateChannel<TStorage, TDomain, Tid>()
        {
            throw new NotImplementedException();
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