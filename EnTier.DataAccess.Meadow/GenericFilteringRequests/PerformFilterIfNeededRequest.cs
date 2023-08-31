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
                    FilterExpression = t.TranslateFilterQueryToDbExpression(filterQuery,FullTreeReadWrite()),
                    ExpirationTimeStamp = typeof(TStorage).GetFilterResultExpirationPointMilliseconds()
                };
            });
        }

        public override string RequestText
        {
            get => Configuration.GetNameConvention<TStorage>().PerformFilterIfNeededProcedureName;
            protected set
            {
                
            }
        }
    }
}