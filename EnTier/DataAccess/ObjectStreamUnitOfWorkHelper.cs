using System.Collections.Generic;
using EnTier.Repositories;

namespace EnTier.DataAccess;

internal static class ObjectStreamUnitOfWorkHelper
{
    internal static List<TStorage> PullOutObjectStreamFromCrudRepositoriesObjectList<TStorage>(IEnumerable<object> repositories)
    {
        var objectsStream = new List<TStorage>();
            
        foreach (var r in repositories)
        {
            var type = r.GetType();

            var genArguments = type.GenericTypeArguments;

            if (genArguments.Length == 2 && genArguments[0] == typeof(TStorage))
            {
                var method = type.GetMethod(nameof(ICrudRepository<object, object>.All));

                if (method != null)
                {
                    var list = method.Invoke(r,new object[] { });

                    if (list is List<TStorage> storageList)
                    {
                        objectsStream = storageList;
                    }
                }
            }
        }

        return objectsStream;
    }
}