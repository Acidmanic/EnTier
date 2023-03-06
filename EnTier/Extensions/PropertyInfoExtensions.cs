using System.Reflection;
using Acidmanic.Utilities.Reflection.Attributes;

namespace EnTier.Extensions;

public static class PropertyInfoExtensions
{


    public static bool IsTreatAsLeaf(this PropertyInfo property)
    {
        return property.GetCustomAttribute<TreatAsLeafAttribute>() != null;
    }
}