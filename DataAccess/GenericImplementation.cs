



using System;
using System.Collections.Generic;
using System.Reflection;

public abstract class GenericImplementation<TContract>
{



    private Type _contractType;
    private Type


    protected Dictionary<Type, Type> ListAllImplementersOf()
        {

            var  ret = new Dictionary<Type, Type>();

            var assembly = Assembly.GetCallingAssembly();

            var types = assembly.GetTypes();

            foreach(var type in types){

                var entity = GetEntityTypeFromImplementedInterface(type);

                if(entity != null){
                    ret.Add(entity,type);
                }
            }

            return ret;
        }


        /// Gets a type, it its generic and 'its 
        protected KeyValuePair GetEntityTypeFromImplementedInterface(Type type)
        {

            if (type.IsInterface || type.IsAbstract){
                return null;
            }
            var parent = type;

            while(parent != null){
                var interfaces = parent.GetInterfaces();

                foreach(var ifc in interfaces){
                    if (ifc.IsGenericType){
                        if (ifc.GetGenericTypeDefinition() == typeof(TContract)){
                            return ifc.GetGenericArguments()[0];
                        }
                    }
                }

                parent = parent.BaseType;
            }

            return null;
        }

        protected Tcast CreateInstance<Tcast>(Type type, object argumet)
        {
            var constructor = type.GetConstructor(new Type[]{argumet.GetType()});

            var ret = constructor.Invoke(new object[]{argumet});

            return (Tcast) ret;
        }
   
}