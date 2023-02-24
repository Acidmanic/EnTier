using Acidmanic.Utilities.Reflection;
using Acidmanic.Utilities.Reflection.Extensions;

namespace EnTier.Utility;

public static class ObjectOperations
{
    public static bool IsGreaterThan<T>(T greater, T smaller)
    {
        if (greater == null)
        {
            return false;
        }

        if (smaller == null)
        {
            return true;
        }

        var type = greater.GetType();
         
        if (TypeCheck.IsReferenceType(type))
        {
            return false;
        }

        if (greater is string sGreater && smaller is string sSmaller)
        {
            return string.CompareOrdinal(sGreater, sSmaller) > 0;
        }
         
        if (greater is char cGreater && smaller is char cSmaller)
        {
            return cGreater > cSmaller;
        }

        if (TypeCheck.IsNumerical(type))
        {
            var gNumber = greater.AsNumber();
            var sNumber = smaller.AsNumber();

            return gNumber > sNumber;
        }

        return false;
    }
}