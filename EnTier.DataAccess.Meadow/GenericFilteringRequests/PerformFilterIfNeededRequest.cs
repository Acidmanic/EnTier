using System;
using Acidmanic.Utilities.DataTypes;
using Acidmanic.Utilities.Filtering;
using Acidmanic.Utilities.Filtering.Extensions;
using Acidmanic.Utilities.Filtering.Models;
using EnTier.DataAccess.Meadow.GenericFilteringRequests.Models;
using Meadow.Contracts;
using Meadow.Extensions;
using Meadow.Requests;

namespace EnTier.DataAccess.Meadow.GenericFilteringRequests
{
    public sealed class PerformFilterIfNeededRequest<TStorage,TId> : MeadowRequest<FilterShell, FilterResult<TId>>
    {
        public PerformFilterIfNeededRequest(
            FilterQuery filterQuery,
            string searchId = null,
            string[] searchTerms = null,
            OrderTerm[] orderTerms=null) : base(true)
        {
            searchId ??= Guid.NewGuid().SearchId();

            searchTerms ??= new string[] { };
            
            orderTerms ??= new OrderTerm[] { };
            
            RegisterTranslationTask(t =>
            {
                var entityType = typeof(TStorage);
                
                var filterExpression = t.TranslateFilterQueryToDbExpression(filterQuery, FullTreeReadWrite());

                var searchExpression = t.TranslateSearchTerm(entityType, searchTerms);

                var orderExpression = t.TranslateOrders(entityType, orderTerms,FullTreeReadWrite());
                
                ToStorage = new FilterShell
                {
                    SearchId = searchId,
                    FilterExpression = filterExpression,
                    SearchExpression = searchExpression,
                    OrderExpression = orderExpression,
                    ExpirationTimeStamp = typeof(TStorage).GetFilterResultExpirationPointMilliseconds()
                };
            });
        }

        public override string RequestText
        {
            get => FullTreeReadWrite()?
                Configuration.GetNameConvention<TStorage>().PerformFilterIfNeededProcedureNameFullTree:
                Configuration.GetNameConvention<TStorage>().PerformFilterIfNeededProcedureName;
            protected set
            {
                
            }
        }
    }
}