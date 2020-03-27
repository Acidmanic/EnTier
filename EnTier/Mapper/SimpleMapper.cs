using Plugging;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using Utility;

namespace Mapper
{
    class SimpleMapper : IObjectMapper
    {
        public TDestination Map<TDestination>(object src)
        {
            var Constructor = ReflectionService.Make().GetConstructorForType<TDestination>(typeof(TDestination));

            if (Constructor.IsNull)
            {
                return default;
            }

            var dst = Constructor.Construct();

            Map(src, dst);

            return dst;
        }

        public void Map<TDestination, TSource>(TSource src, TDestination dst) {
            Map(src, dst);
        }

        public void Map(object src, object dst)
        {
            var srcType = src.GetType();
            var dstType = dst.GetType();

            var srcProps = srcType.GetProperties();
            var dstProps = dstType.GetProperties();

            foreach(var p in dstProps)
            {
                var srcP = FindEqual(srcProps, p);

                if(srcP != null)
                {
                    Set(srcP,src, p,dst);
                }
            }

        }

        private bool Equals(PropertyInfo p1,PropertyInfo p2)
        {
            if (p1.PropertyType != p2.PropertyType)
            {
                return false;
            }
            var caseless1 = UnCase(p1.Name);

            var caseless2 = UnCase(p2.Name);

            return caseless1 == caseless2;
        }

        private string UnCase(string name)
        {
            var ret = name;

            ret = ret.Replace("_", "");

            ret = ret.Replace("-", "");

            ret = ret.ToLower();

            return ret;
        }

        private PropertyInfo FindEqual(PropertyInfo[] properties,PropertyInfo search)
        {
            foreach(var prop in properties)
            {
                if (Equals(prop, search))
                {
                    return prop;
                }
            }

            return null;
        }

        private void Set(PropertyInfo srcProp,object src,PropertyInfo dstProp,object dst)
        {
            try
            {
                var value = srcProp.GetValue(src);
                
                dstProp.SetValue(dst,value);
            }
            catch (Exception){            }
        }

        
    }
}
