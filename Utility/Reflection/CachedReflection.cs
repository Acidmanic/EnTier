



using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Utility{




    public class CachedReflection{


        private List<MetaType> _types;

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
                        prop.DeclaringType,
                        () => prop.GetValue(obj)
                    ));
                }
            }

            return this;
        }

        public CachedReflection CachePropertyTypes(Object obj){
            
            var properties = obj.GetType().GetProperties();

            foreach(var prop in properties){
                _types.Add(new MetaType(prop.DeclaringType));
            }

            return this;
        }



        public List<Type> GetTypesWhichImplement(Type type){
            
            return _types.Select(t => t.Type)
                         .Where(t => Implements(t,type))
                         .ToList();
        }

        public Func<TCast> GetCreatorForTypeWhichImplements<TCast>(){

            var type = typeof(TCast);

            return GetCreatorForTypeWhich<TCast>(
                mt => Extends(mt.Type,type)
            );
        }

        public Func<TCast> GetCreatorForTypeWhichExtends<TCast>()
        {
            var type = typeof(TCast);

            return GetCreatorForTypeWhich<TCast>(
                mt => Implements(mt.Type,type)
            );
        }

        private Func<TCast> GetCreatorForTypeWhich<TCast>(Func<MetaType,bool> predicate){
            
            var res = _types.Where(predicate)
                            .Select(mt => mt.Instanciator)
                            .FirstOrDefault();
                            
            if (res != null){
                return () => (TCast)res();
            }

            return () => default;
        }

        public Type[] GetTypes(params Object[] objects){

            var ret = new Type[objects.Length];

            for(int i=0;i<objects.Length;i++){
                ret[i] = objects[i].GetType();
            }

            return ret;

        }

        public Func<TCast> FindConstructor<TCast>(
                Func<Type,bool> predicate,
                params Object[] argumets
        ){
            var creators = _types.FindAll(mt => predicate(mt.Type));

            if (creators != null){

                foreach(var c in creators){
                    var type = c.Type;
                    var argTypes = GetTypes(argumets);
                    var constructor = type.GetConstructor(argTypes);

                    if (constructor!=null){
                        return () => (TCast) constructor.Invoke(argumets);
                    }
                }
            }
            return null;
        }

        public Func<TCast> FindConstructor<TCast>(params Object[] argumets){
            var type = typeof(TCast);

            return FindConstructor<TCast>(
                t => Implements(t,type)
                ,argumets
            );
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

        public bool Extends(Type t, Type type)
        {
            if(t == type) return true;

            var parent = t.BaseType;

            if(parent != null){
                return Extends(parent,type);
            }

            return false;
        }

    }
}