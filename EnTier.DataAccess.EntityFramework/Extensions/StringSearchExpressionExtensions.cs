// using System;
// using System.Collections.Generic;
// using System.Linq;
// using System.Linq.Expressions;
// using Acidmanic.Utilities.Filtering.Models;
// using EnTier.DataAccess.JsonFile;
// using Microsoft.EntityFrameworkCore;
//
// namespace EnTier.DataAccess.EntityFramework.Extensions;
//
// public static class StringSearchExpressionExtensions
// {
//
//
//
//     public static Expression<Func<TStorage,MarkedSearchIndex<TStorage,TId>,bool>>
//             ToSearchExpression<TStorage,TId>(this string[] terms)
//     {
//     
//         // stg
//         ParameterExpression storageExpression = Expression.Parameter(typeof(TStorage), "stg");
//         // six
//         ParameterExpression searchIndexExpression = Expression.Parameter(typeof(TId), "six");
//         
//         
//         Expression<Func<TStorage, MarkedSearchIndex<TStorage, TId>, bool>> expression = (st, si) => true;
//     
//         Expression leftOperand = () => true;
//     
//         //six.IndexCorpus
//         MemberExpression sixIndexCorpus = Expression.Property(searchIndexExpression,
//             nameof(MarkedSearchIndex<TStorage, TId>.IndexCorpus));
//         
//         var shit = Expression.con
//         
//         foreach (var term in terms)
//         {
//             
//             Expression<Func<MarkedSearchIndex<TStorage, TId>, bool>> rightOperand = si => si.IndexCorpus.Contains(term);
//     
//             leftOperand = Expression.And(leftOperand, rightOperand);
//         }
//     
//     
//         return expressions;
//     }
// }