using System;
using System.Linq.Expressions;
using Acidmanic.Utilities.Reflection.ObjectTree;
using Acidmanic.Utilities.Reflection.ObjectTree.FieldAddressing;

namespace EnTier.Extensions
{
    internal static class TypeExtensions
    {
        /// <summary>
        /// Checks if the given type is able to be instantiated using new keyword. 
        /// </summary>
        /// <param name="type">Type to be checked.</param>
        /// <returns>True if the type is newable, False otherwise.</returns>
        public static bool IsNewable(this Type type)
        {
            if (type.IsAbstract || type.IsInterface)
            {
                return false;
            }

            var constructor = type.GetConstructor(new Type[] { });

            if (constructor == null)
            {
                return false;
            }

            return true;
        }


    }
}