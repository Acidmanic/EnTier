using System;
using Acidmanic.Utilities.Filtering;
using Acidmanic.Utilities.Filtering.Extensions;
using Acidmanic.Utilities.Filtering.Models;
using EnTier.DataAccess.Meadow.GenericFilteringRequests.Models;
using Meadow.Contracts;
using Meadow.Requests;

namespace EnTier.DataAccess.Meadow.GenericFilteringRequests
{
    public sealed class PerformFilterIfNeededRequest<TStorage> : MeadowRequest<FilterShell, FilterResult>
    {
        public PerformFilterIfNeededRequest(FilterQuery filterQuery) : base(true)
        {
            RegisterTranslationTask(t =>
            {
                ToStorage = new FilterShell
                {
                    FilterHash = filterQuery.Hash(),
                    FilterExpression = t.TranslateFilterQueryToWhereClause(filterQuery),
                    ExpirationTimeStamp = DateTime.Now.Ticks + typeof(TStorage).GetFilterResultExpirationTimeSpan().Ticks
                };
            });
        }

        public override string RequestText
        {
            get => new NameConvention<TStorage>().PerformFilterIfNeededProcedureName;
            protected set
            {
                
            }
        }
    }
}