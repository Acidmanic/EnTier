


using System;
using System.Collections.Generic;
using System.Reflection;

namespace Utility{


    public class Reflection{


        public static List<T> GetTypeAttributes<T>(object obj){
            var type = obj.GetType();

            var attributes = type.GetCustomAttributes(true);

            return FilterByType<T>(attributes);
        }

        public static List<T> GetAttributes<T>(MethodBase methodInfo){

            var attributes = methodInfo.GetCustomAttributes(false);

            return FilterByType<T>(attributes);
        }

        private static List<T> FilterByType<T>(object[] attributes)
        {
            var selected = new List<T>();

            foreach(var att in attributes){
                if (att is T casted)
                {
                    selected.Add(casted);
                }
            }

            return selected;
        }

        public static Func<Entity,Object> GetPropertyReader<Entity>(Type type,string propName){
            
            var property = type.GetProperty(propName);

            if (property != null){
                return (Entity obj) => property.GetValue(obj);
            }

            return null;
        }
    }
}