using Acidmanic.Utilities.Reflection.ObjectTree.FieldAddressing;

namespace EnTier.Extensions;

public static  class FieldKeyExtensions
{




    public static FieldKey SubKey(this FieldKey key, int startIndex, int length)
    {
        var subKey = new FieldKey();

        for (int i = startIndex; i < startIndex + length; i++)
        {
            subKey.Add(key[i]);
        }

        return subKey;
    }
    
}