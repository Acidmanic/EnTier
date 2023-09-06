using Acidmanic.Utilities.Filtering.Models;

namespace EnTier.DataAccess.JsonFile;

public class MarkedSearchIndex<TStorage,TId>:SearchIndex<TId>
{

}

public static class SearchIndexEfMarkingExtensions
{
    public static MarkedSearchIndex<TStorage, TId> AsMarked<TStorage,TId>(this SearchIndex<TId> value)
        {
            return new MarkedSearchIndex<TStorage, TId>
            {
                Id = value.Id,
                ResultId = value.ResultId,
                IndexCorpus = value.IndexCorpus
            };
        }
}