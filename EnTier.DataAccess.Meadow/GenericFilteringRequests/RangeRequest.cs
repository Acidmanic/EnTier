using EnTier.DataAccess.Meadow.GenericFilteringRequests.Models;
using EnTier.Models;
using Meadow.Extensions;
using Meadow.Requests;

namespace EnTier.DataAccess.Meadow.GenericFilteringRequests
{
    public sealed  class RangeRequest<TEntity>:MeadowRequest<FieldNameShell,FieldRange>
    {
        public RangeRequest(string fieldName) : base(true)
        {
            RegisterTranslationTask(t =>
            {
                ToStorage = new FieldNameShell
                {
                    FieldName = t.TranslateFieldName(typeof(TEntity),fieldName,FullTreeReadWrite())
                };
            });
        }
        
        public override string RequestText
        {
            get => Configuration.GetNameConvention(typeof(TEntity)).Range;
            protected set
            {
                
            }
        }
    }
}