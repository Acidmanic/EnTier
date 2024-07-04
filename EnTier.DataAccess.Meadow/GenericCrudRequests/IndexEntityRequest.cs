using Acidmanic.Utilities.Filtering.Models;
using Meadow.Extensions;
using Meadow.Requests;

namespace EnTier.DataAccess.Meadow.GenericCrudRequests
{
    public sealed class IndexEntityRequest<TEntity,TId> : MeadowRequest<SearchIndex<TId>, SearchIndex<TId>> 
    {
        public IndexEntityRequest(string corpus,TId id) : base(true)
        {
            ToStorage = new SearchIndex<TId>()
            {
                IndexCorpus = corpus,
                ResultId = id
            };
        }

        // protected override void OnFieldManipulation(IFieldInclusionMarker toStorage, IFieldInclusionMarker fromStorage)
        // {
        //     base.OnFieldManipulation(toStorage, fromStorage);
        //     
        //     var idLeaf = TypeIdentity.FindIdentityLeaf<SearchIndex<TId>>();
        //         
        //     toStorage.Exclude(idLeaf.GetFullName());
        //     
        // }

        public override string RequestText
        {
            get
            {
                var nc = Configuration.GetNameConvention<TEntity>();

                return nc.IndexEntityProcedureName;
            }
        }
    }
}