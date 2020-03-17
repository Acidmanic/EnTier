
using System;
using System.Reflection;

namespace DataAccess{




    public class MethodAttributesEagerScope<T> : IDisposable
    where T:class
    {

        private readonly EagerScopeManager _manager ;

        public MethodAttributesEagerScope(MethodBase methodInfo)
        {
            _manager = new EagerAttributeProcessor().MarkEagers<T>(methodInfo);


        }
        public void Dispose()
        {
            if(_manager!=null){
                _manager.Dispose();
            }
        }
    }
}