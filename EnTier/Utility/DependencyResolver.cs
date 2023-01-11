using System.Collections.Generic;
using System.Linq;
using EnTier.Exceptions;

namespace EnTier.Utility
{
    /// <summary>
    /// This class takes a list of dependency map in the format of dictionary of lists, which pairs each object
    ///  with it's dependencies. It will return an ordered array of keys from list dependant to most dependant object. 
    /// </summary>
    /// <typeparam name="T">Type of objects that are dependent to each other. it kan be a key or actual object. If
    /// this type fully supports equality check methods (==, Equals and hash equality) it would work fine. </typeparam>
    public class DependencyResolver<T>
    {
        private class DependencyMap : Dictionary<T, List<T>>
        {
            public override string ToString()
            {
                var view = "";

                foreach (var item in this)
                {
                    view += item.Key.ToString() + ": ";

                    var sep = "";

                    foreach (var v in item.Value)
                    {
                        view += sep + v.ToString();

                        sep = ",";
                    }

                    view += "\n";
                }

                return view;
            }
        }

        public T[] OrderByDependency(Dictionary<T, List<T>> map)
        {
            var convertedMap = new DependencyMap();

            foreach (var item in map)
            {
                convertedMap.Add(item.Key, item.Value);
            }

            var orderedList = OrderByDependency(convertedMap);

            return orderedList;
        }

        private T[] OrderByDependency(DependencyMap map)
        {
            var fullMap = GetFullMap(map);

            var keys = fullMap.Keys.ToArray();

            var sorted = false;

            while (!sorted)
            {
                sorted = true;

                for (int i = 0; i < keys.Length - 1; i++)
                {
                    var key = keys[i];

                    var currentDependencies = map[key];

                    if (DependsOnRest(keys, i, currentDependencies))
                    {
                        sorted = false;

                        (keys[i], keys[i + 1]) = (keys[i + 1], keys[i]);
                    }
                }
            }

            return keys;
        }

        private bool DependsOnRest(T[] keys, int index, List<T> currentDependencies)
        {
            for (int i = index + 1; i < keys.Length; i++)
            {
                var key = keys[i];

                if (currentDependencies.Contains(key))
                {
                    return true;
                }
            }

            return false;
        }


        private DependencyMap GetFullMap(DependencyMap markedMap)
        {
            var fullMap = new DependencyMap();

            foreach (var key in markedMap.Keys)
            {
                var allDependencies = new List<T>();

                FindFullDependencies(key, key, markedMap, allDependencies);

                fullMap.Add(key, allDependencies);
            }

            return fullMap;
        }


        private void FindFullDependencies(T unrecursedKey, T key, DependencyMap map, List<T> result)
        {
            var dependencies = map[key];

            foreach (var dependency in dependencies)
            {
                if (AreEqual(unrecursedKey, dependency))
                {
                    throw new DependencyLoopException(unrecursedKey.ToString());
                }

                if (!result.Contains(dependency))
                {
                    result.Add(dependency);
                }

                FindFullDependencies(unrecursedKey, dependency, map, result);
            }
        }


        private bool AreEqual(T a, T b)
        {
            if (a == null && b == null)
            {
                return true;
            }

            if (a == null || b == null)
            {
                return false;
            }

            return a.Equals(b);
        }
    }
}