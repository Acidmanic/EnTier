


using System;
using System.Collections.Generic;

namespace Utility{


    public class Reflection{


        public static List<T> GetAttributes<T>(object obj){
            var type = obj.GetType();

            var attributes = type.GetCustomAttributes(true);

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