using EnTier.Reflection;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("EnTier.DataAccess.EntityFramework")]
namespace EnTier.Utility
{
    internal class Reflection
    {
        public static List<T> GetTypeAttributes<T>(object obj)
        {
            var type = obj.GetType();

            var attributes = type.GetCustomAttributes(true);

            return FilterByType<T>(attributes);
        }

        public static List<T> GetAttributes<T>(MethodBase methodInfo)
        {
            var attributes = methodInfo.GetCustomAttributes(true);

            return FilterByType<T>(attributes);
        }

        private static List<T> FilterByType<T>(object[] attributes)
        {
            var selected = new List<T>();

            foreach (var att in attributes)
            {
                if (att is T casted)
                {
                    selected.Add(casted);
                }
            }

            return selected;
        }

        public static Func<Entity, TProperty> GetPropertyReader<Entity, TProperty>(string propName)
        {
            var type = typeof(Entity);

            var property = type.GetProperty(propName);

            if (property != null)
            {
                return (Entity obj) => (TProperty) property.GetValue(obj);
            }

            return null;
        }

        public static Action<TEntity, TProperty> GetPropertyWriter<TEntity, TProperty>(string propName)
        {
            var type = typeof(TEntity);

            var property = type.GetProperty(propName);

            if (property != null)
            {
                return (TEntity obj, TProperty value) => property.SetValue(obj, value);
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
                ret = new PropertyWrapper<T>(propInfo, obj);
            }

            return ret;
        }
    }
}