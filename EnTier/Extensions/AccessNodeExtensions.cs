using System;
using System.Linq;
using Acidmanic.Utilities.Filtering.Attributes;
using Acidmanic.Utilities.Reflection;
using Acidmanic.Utilities.Reflection.Attributes;
using Acidmanic.Utilities.Reflection.ObjectTree;

namespace EnTier.Extensions;

public static class AccessNodeExtensions
{
    /// <summary>
    /// Checks if the leaf has altered type, If so returns that. otherwise returns the leaf's node.
    /// Throws exception if the given node is not a Leaf node.
    /// </summary>
    /// <param name="leaf">The leaf which it's type is requested</param>
    /// <returns>The effective type of the node for data access purposes</returns>
    public static Type GetEffectiveType(this AccessNode leaf)
    {
        if (!leaf.IsLeaf)
        {
            throw new Exception("The given node must be a leaf.");
        }

        var foundAttribute = leaf.PropertyAttributes
            .FirstOrDefault(a => a is AlteredTypeAttribute);

        if (foundAttribute is AlteredTypeAttribute altered)
        {
            return altered.AlternativeType;
        }

        return leaf.Type;
    }


    
    
    public static string GetEffectiveEs6Type(this AccessNode leaf)
    {
        var type = leaf.GetEffectiveType();

        if (TypeCheck.IsNumerical(type))
        {
            return "number";
        }

        if (type == typeof(bool))
        {
            return "boolean";
        }

        if (type == typeof(DateTime))
        {
            return "date";
        }

        return "string";
    }


    public static bool IsFilterField(this AccessNode node)
    {
        return node.PropertyAttributes.Any(a => a is FilterFieldAttribute);
    }
}