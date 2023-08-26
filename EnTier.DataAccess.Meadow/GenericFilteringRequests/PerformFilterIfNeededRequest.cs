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
    public sealed class PerformFilterIfNeededRequest<TStorage> : MeadowRequest<FilterShell, FilterResult>
    {
        public PerformFilterIfNeededRequest(FilterQuery filterQuery,string searchId = null) : base(true)
        {
            searchId ??= Guid.NewGuid().SearchId();
            
            RegisterTranslationTask(t =>
            {
                ToStorage = new FilterShell
                {
                    SearchId = searchId,
                    FilterExpression = t.TranslateFilterQueryToWhereClause(filterQuery),
                    ExpirationTimeStamp = TimeStamp.Now.TotalMilliSeconds  + 
                                          ((TimeStamp)typeof(TStorage).GetFilterResultExpirationTimeSpan())
                                          .TotalMilliSeconds
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