



using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Utility{




    public class CachedReflection:Reflection{


        private readonly List<MetaType> _types;

        public CachedReflection()
        {
            _types = new List<MetaType>();
            
        }


        private void AddRange(ICollection<Type> types){
            types.ToList().ForEach( t => _types.Add( new MetaType(t)));
        }

        public CachedReflection CacheCurrent(){
            var assembly = Assembly.GetCallingAssembly();
            AddRange(assembly.GetTypes());
            return this;
        }

        public CachedReflection Cache(Assembly assembly){
            AddRange(assembly.GetTypes());
            return this;
        }

        

        public CachedReflection CachePropertiesOf(Object obj){
            
            var properties = obj.GetType().GetProperties();

            foreach(var prop in properties){
                if(prop.CanRead){
                    _types.Add( new MetaType(
                        prop.PropertyType,
                        () => prop.GetValue(obj)
                    ));
                }
            }

            return this;
        }

        public CachedReflection CachePropertyTypes(Object obj){
            
            var properties = obj.GetType().GetProperties();

            foreach(var prop in properties){
                _types.Add(new MetaType(prop.PropertyType));
            }

            return this;
        }



        public List<Type> GetTypesWhichImplement(Type type){
            
            return _types.Select(t => t.Type)
                         .Where(t => Implements(t,type))
                         .ToList();
        }


        public bool IsAnyExtensionFor<T>(){

            var type = typeof(T);

            return _types.Select(t => t.Type)
                         .Where(t => Extends(t,type))
                         .Count()>0;
        }


        public bool IsAnyImplementationFor<T>(){

            var type = typeof(T);

            return _types.Select(t => t.Type)
                         .Where(t => Implements(t,type))
                         .Count()>0;
        }

        public Constructor<TCast> GetCreatorForTypeWhichImplements<TCast>(){

            var type = typeof(TCast);

            return GetCreatorForTypeWhich<TCast>(
                mt => Implements(mt.Type,type)
            );
        }

        public Constructor<TCast> GetCreatorForTypeWhichExtends<TCast>()
        {
            var type = typeof(TCast);

            return GetCreatorForTypeWhich<TCast>(
                mt => Extends(mt.Type,type)
            );
        }

        private Constructor<TCast> GetCreatorForTypeWhich<TCast>(Func<MetaType,bool> predicate){
            
            var res = _types.Where(predicate)
                            .Select(mt => mt.Instanciator)
                            .FirstOrDefault();
                            
            if (res != null){
                return new Constructor<TCast>(() => (TCast)res());
            }

            return Constructor<TCast>.Null();
        }

        public Type[] GetTypes(params Object[] objects){

            var ret = new Type[objects.Length];

            for(int i=0;i<objects.Length;i++){
                ret[i] = objects[i].GetType();
            }

            return ret;

        }

        public Constructor<TCast> FindConstructor<TCast>(
                Func<Type,bool> predicate,
                params Object[] arguments
        ){
            var creators = _types.FindAll(mt => predicate(mt.Type));

            if (creators != null){

                foreach(var c in creators){
                    var res = GetConstructorForType<TCast>(c.Type,arguments);
                    
                    if(!res.IsNull) return res;
                }
            }
            return Constructor<TCast>.Null();
        }

        public Constructor<TCast> GetConstructorForType<TCast>(Type type,params object[] arguments){

                    var argTypes = GetTypes(arguments);

                    var constructor = type.GetConstructor(argTypes);

                    if (constructor!=null){
                        return new Constructor<TCast>(constructor,arguments);
                    }

                    return Constructor<TCast>.Null();
        }

        public Constructor<TCast> FindConstructor<TCast>(params Object[] argumets){
            var type = typeof(TCast);

            return FindConstructor<TCast>(
                t => Implements(t,type)
                ,argumets
            );
        }

        public Constructor<TInterface> FindConstructorByPriority<TInterface,TLowPrType>(params Object[] arguments){
            var type = typeof(TInterface);
            var exclude = typeof(TLowPrType);
            var ret =  FindConstructor<TInterface>(
                t => t != exclude && Implements(t,type) 
                ,arguments
            );

            if(ret.IsNull){
                ret = GetConstructorForType<TInterface>(exclude,arguments);
            }
            
            return ret;
        }

        public bool Implements(Type t, Type type)
        {
            var ifaces = t.GetInterfaces();

            foreach(var iface in ifaces){
                if(iface.Equals(type)) return true;
            }

            var parent = t.BaseType;

            if(parent != null) return Implements(parent,type);

            return false;
        }

        public bool Extends(Type t, Type @base)
        {
            if(t == @base) return true;

            var parent = t.BaseType;

            if(parent != null){
                return Extends(parent,@base);
            }

            return false;
        }

    }
}