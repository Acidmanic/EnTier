


using System;
using System.Collections.Generic;
using Controllers;
using DIBinding;
using Microsoft.AspNetCore.Mvc;
using Plugging;
using Repository;
using Service;
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


        private string GetKey<T1,T2,T3>(){

            string ret = typeof(T1).GUID.ToString() + ":"
                       + typeof(T2).GUID.ToString() + ":"
                       + typeof(T3).GUID.ToString();
            return ret;

        }

        public ResolvedChannel<TStorage,TDomain,TId> ResolveChannel<TStorage,TDomain,TId>()
        where TStorage:class
        {

            var key = GetKey<TStorage,TDomain,TId>();
            
            var ret = new ResolvedChannel<TStorage,TDomain,TId>(key);

            var factory = new BuilderFactory<TStorage,TDomain,TId>();

            ret.Service = factory.MakeBuilder<IService<TDomain,TId>>();

            ret.Repository = factory.MakeBuilder<IRepository<TStorage,TId>>();

            return ret;
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
                    
                }

            }

        }
    }
}