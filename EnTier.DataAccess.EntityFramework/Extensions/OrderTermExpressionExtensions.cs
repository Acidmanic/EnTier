using System;
using System.Linq.Expressions;
using Acidmanic.Utilities.Filtering.Models;

namespace EnTier.DataAccess.EntityFramework.Extensions;

public static class OrderTermExpressionExtensions
{
    public static Expression<Func<TStorage, object>> GetSelector<TStorage>(this OrderTerm term)
    {
        var type = typeof(TStorage);

        var segments = term.Key.Split(".");
        //s
        ParameterExpression storageParameter = Expression.Parameter(type, "s");
        //s.<segments[0]>
        Expression member = Expression.Property(storageParameter, segments[0]);
        //s.<address>
        for (int i = 1; i < segments.Length; i++)
        {
            member = Expression.Property(member, segments[i]);
        }

        //s=> s.<address>
        var lambda = Expression.Lambda<Func<TStorage, Object>>(member, storageParameter);

        return lambda;
    }
    
}