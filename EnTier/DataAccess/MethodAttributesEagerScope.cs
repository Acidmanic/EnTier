
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using EnTier.Utility;

namespace EnTier.DataAccess
{




    public class MethodAttributesEagerScope<T> : IDisposable
    where T:class
    {

        private readonly List<EagerScopeManager> _managers = new List<EagerScopeManager>() ;

        public MethodAttributesEagerScope(object caller)
        {
            
            var callerType = caller.GetType();

            var methods = GetMethodsBehind(t => ReflectionService.Make()
                                           .Extends(t,callerType));

            foreach(var method in methods){
                var manager = new EagerAttributeProcessor().MarkEagers<T>(method);

                if(manager!=null){
                    _managers.Add(manager);
                }
            }

        }

        private List<MethodBase> GetMethodsBehind(Func<Type,bool> predicate)
        {
            var ret = new List<MethodBase>();

            var trace = new StackTrace();
            


            for(int i=0;i<trace.FrameCount+1;i++){
                var method = trace.GetFrame(i).GetMethod();
                var declaringType = method.DeclaringType;

                if(declaringType==null) break;

                ret.Add(method);
                
                if(predicate(method.DeclaringType)) break;
            }
            
            ret.Reverse();

            return ret;
        }

        public void Dispose()
        {
            foreach(var manager in _managers){
                manager.Dispose();
            }
        }
    }
}