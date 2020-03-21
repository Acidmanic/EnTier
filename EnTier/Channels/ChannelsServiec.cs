


using System;
using System.Collections.Generic;
using Controllers;
using Microsoft.AspNetCore.Mvc;
using Utility;

namespace Channels{

    public class ChannelsService{



        private static ChannelsService instance = null;

        private ChannelsService(){
            UpdateChannels();
        }

        public static ChannelsService Make(){
            var obj = new object();

            lock(obj){

                if(instance == null){
                    instance = new ChannelsService();
                }
            }

            return instance; 
        }


        private class _{}

        private Dictionary<Type,Channel> _channels;


        private string NonGenericFullNameOf<T>(){
            return NonGenericFullNameOf(typeof(T));
        }
        private string NonGenericFullNameOf(Type type){
            string name = type.FullName;
            int st = name.IndexOf("`");
            if(st <= 0) return name;
            name = name.Substring(0,st);
            return name;
        }

        public void UpdateChannels()
        {
            _channels = new Dictionary<Type, Channel>();

            var controllers = ReflectionService.Make()
                .GetTypesWhichImplement<IEnTierController>();

            var baseName = NonGenericFullNameOf<EnTierControllerBase<_,_,_,_>>();

            foreach(var controller in controllers){

                if(!controller.IsAbstract){
                    var channel = new Channel();

                    var cType = ReflectionService.Make().GetAncesstor(controller,
                        t => NonGenericFullNameOf(t) == baseName, 
                        t => t.IsGenericType,
                        t => t.GenericTypeArguments.Length == 4
                    );

                    channel.ControllerType = controller;

                    _channels.Add(controller,channel);
                    
                }

            }

        }
    }
}