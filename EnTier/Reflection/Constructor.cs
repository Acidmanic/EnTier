



using System;
using System.Diagnostics;
using System.Reflection;

namespace Utility{




    public sealed class Constructor<T> 
    {
        

        private readonly Func<T> _constructor;

        public Constructor(Func<T> constructor)
        {
            _constructor = constructor;
            IsNull = false;
        }

        public Constructor(ConstructorInfo constructor, params Object[] arguments){

            _constructor = () =>  (T) constructor.Invoke(arguments);

            IsNull = false;
        }

        private Constructor()
        {
            _constructor = ()=> default;
        }

        public static Constructor<T> Null(){
            var ret = new Constructor<T>();
            ret.IsNull = true;
            return ret;
        }

        public bool IsNull{get;private set;}

        public T Construct(){
            return _constructor();
        }

    }
}