using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using EnTier.Reflection;
using EnTier.Utility;

namespace EnTier.Mapper
{
    public class EntierBuiltinMapper : IMapper
    {
        public TDestination Map<TDestination>(object src)
        {
            if (ReflectionService.Make().Implements<IEnumerable,TDestination>())
            {
                var type = typeof(TDestination);

                var entityType = type.GenericTypeArguments[0];
                
                var list = new DynamicList(entityType);

                var srcEnumerable = (IEnumerable) src;

                var constructor = ReflectionService.Make().GetConstructorForType(entityType);
                
                foreach (var item in srcEnumerable)
                {

                    var entity = constructor.Construct();
                    
                    MapProperties(item,entity);
                    
                    list.Add(entity);
                }

                return list.Cast<TDestination>();
            }

            return MapSingleObject<TDestination>(src);
        }
        
        
        private TDestination MapSingleObject<TDestination>(object src)
        {
            var Constructor = ReflectionService.Make().GetConstructorForType<TDestination>(typeof(TDestination));

            if (Constructor.IsNull)
            {
                return default;
            }

            var dst = Constructor.Construct();

            MapByObjects(src, dst);

            return dst;
        }

        public void Map<TDestination, TSource>(TSource src, TDestination dst)
        {
            MapByObjects(src, dst);
        }

        private void MapByObjects(object src, object dst)
        {
            _ = MapAsDictionaries(src, dst)
                ||
                MapAsList(src, dst);

            MapProperties(src, dst);
        }

        private bool MapAsDictionaries(object src, object dst)
        {
            if (src is IDictionary && dst is IDictionary)
            {
                var srcAsDic = (IDictionary) src;
                var dstAsDic = (IDictionary) dst;

                var dstType = dst.GetType();

                var keyvalueTypes = GetGenericsOf(dstType, typeof(IDictionary<,>));

                if (keyvalueTypes.Length == 2)
                {
                    var dstKeyMaker = ReflectionService.Make().GetConstructorForType<object>(keyvalueTypes[0]);
                    var dstValueMaker = ReflectionService.Make().GetConstructorForType<object>(keyvalueTypes[1]);

                    foreach (var sKey in srcAsDic.Keys)
                    {
                        var sValue = srcAsDic[sKey];

                        var dKey = dstKeyMaker.Construct();

                        var dValue = dstValueMaker.Construct();

                        MapByObjects(sKey, dKey);

                        MapByObjects(sValue, dValue);

                        dstAsDic.Add(dKey, dValue);
                    }
                }

                return true;
            }

            return false;
        }

        private Type[] GetGenericsOf(Type dType, Type genericInterface)
        {
            var all = dType.GetInterfaces();

            foreach (var iface in all)
            {
                if (ReflectionService.Make().IsSpecificOf(iface, genericInterface))
                {
                    if (iface.IsGenericType)
                    {
                        return iface.GenericTypeArguments;
                    }
                    else
                    {
                        break;
                    }
                }
            }

            return new Type[] { };
        }

        private bool MapAsList(object src, object dst)
        {
            if (src is IEnumerable && dst is IList)
            {
                var srcEnum = (IEnumerable) src;
                var dstList = (IList) dst;

                var dType = dst.GetType();

                if (dType.IsGenericType)
                {
                    var dgTypes = GetGenericsOf(dType, typeof(IList<>));

                    if (dgTypes.Length > 0)
                    {
                        var dConst = ReflectionService.Make().GetConstructorForType<object>(dgTypes[0]);

                        foreach (var s in srcEnum)
                        {
                            var d = dConst.Construct();

                            if (d != null)
                            {
                                MapByObjects(s, d);

                                dstList.Add(d);
                            }
                        }

                        return true;
                    }
                }
            }

            return false;
        }

        private void MapProperties(object src, object dst)
        {
            var srcType = src.GetType();
            var dstType = dst.GetType();

            var srcProps = srcType.GetProperties();
            var dstProps = dstType.GetProperties();

            foreach (var p in dstProps)
            {
                var srcP = FindEqual(srcProps, p);

                if (srcP != null)
                {
                    Set(srcP, src, p, dst);
                }
            }
        }

        private bool Equals(PropertyInfo p1, PropertyInfo p2)
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

        private PropertyInfo FindEqual(PropertyInfo[] properties, PropertyInfo search)
        {
            foreach (var prop in properties)
            {
                if (Equals(prop, search))
                {
                    return prop;
                }
            }

            return null;
        }

        private void Set(PropertyInfo srcProp, object src, PropertyInfo dstProp, object dst)
        {
            try
            {
                var value = srcProp.GetValue(src);

                dstProp.SetValue(dst, value);
            }
            catch (Exception)
            {
            }
        }
    }
}