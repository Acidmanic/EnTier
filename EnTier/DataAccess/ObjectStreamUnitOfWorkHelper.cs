using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Acidmanic.Utilities.Reflection;
using Acidmanic.Utilities.Reflection.ObjectTree;
using EnTier.Repositories;
using EnTier.UnitOfWork;

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
    
    //
    // public static TStorage ProvideFullTree<TStorage>(IUnitOfWork osUnitOfWork, TStorage value) where TStorage : class
    // {
    //     var ev = new ObjectEvaluator(value);
    //
    //     return ProvideFullTree(osUnitOfWork, value,typeof(TStorage)) as TStorage;
    // }
    //
    // public static object ProvideFullTree(IUnitOfWork osUnitOfWork, object value,Type storageType)
    // {
    //     var ev = new ObjectEvaluator(value);
    //
    //     var modelNodes = ev.RootNode.GetChildren()
    //         .Where(IsModelChild);
    //
    //     foreach (var node in modelNodes)
    //     {
    //         var 
    //     }
    // }
    //
    //
    // private static bool IsModelChild(AccessNode node)
    // {
    //     var type = node.Type;
    //
    //     return !node.IsCollection && !TypeCheck.IsEffectivelyPrimitive(type) &&  TypeCheck.IsModel(type);
    // }

}