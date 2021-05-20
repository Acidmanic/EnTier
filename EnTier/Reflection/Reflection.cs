


using EnTier.Reflection;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace EnTier.Utility{


    internal class Reflection{


        public static List<T> GetTypeAttributes<T>(object obj){
            var type = obj.GetType();

            var attributes = type.GetCustomAttributes(true);

            return FilterByType<T>(attributes);
        }

        public static List<T> GetAttributes<T>(MethodBase methodInfo){

            var attributes = methodInfo.GetCustomAttributes(true);

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

        public static Func<Entity,TId> GetPropertyReader<Entity,TId>(string propName)
        {
            var type = typeof(Entity);
            
            var property = type.GetProperty(propName);

            if (property != null){
                return (Entity obj) => (TId) property.GetValue(obj);
            }

            return null;
        }

        public static PropertyWrapper<T> GetProperty<T>(object obj, string propertyName)
        {
            var ret = new PropertyWrapper<T>();

            var type = obj.GetType();

            var propInfo = type.GetProperty(propertyName);

            if (propInfo != null)
            {
                ret = new PropertyWrapper<T>(propInfo,obj);
            }
            return ret;
        }
    }
}