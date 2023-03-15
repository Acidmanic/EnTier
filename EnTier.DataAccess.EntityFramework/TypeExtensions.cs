using System;
using System.Collections.Generic;

namespace EnTier.DataAccess.EntityFramework;

public static class TypeExtensions
{
    public static List<Type> GetBaseTypesHierarchy(this Type type)
    {
        var hierarchy = new List<Type>();

        var currentType = type.BaseType;

        while (currentType != null)
        {
            hierarchy.Add(currentType);

            currentType = currentType.BaseType;
        }

        return hierarchy;
    }
    
    
    
}