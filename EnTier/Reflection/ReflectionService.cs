using System;
using System.Linq;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("EnTier.DataAccess.EntityFramework")]

namespace EnTier.Utility
{
    internal class ReflectionService : CachedReflection
    {
        private static ReflectionService instance = null;

        private ReflectionService()
        {
            CacheCurrent();
        }

        public static ReflectionService Make()
        {
            var obj = new object();

            lock (obj)
            {
                if (instance == null)
                {
                    instance = new ReflectionService();
                }

                instance.ClearFilters();
            }

            return instance;
        }
    }
}