using Acidmanic.Utilities.Filtering.Models;

namespace EnTier.DataAccess.EntityFramework.Models;

public class MarkedFilterResult<TStorage,TId>:FilterResult<TId>
{

}

public static class FilterResultsEfMarkingExtensions
{
    public static MarkedFilterResult<TStorage, TId> AsMarked<TStorage,TId>(this FilterResult<TId> value)
        {
            return new MarkedFilterResult<TStorage, TId>
            {
                Id = value.Id,
                ResultId = value.ResultId,
                SearchId = value.SearchId,
                ExpirationTimeStamp = value.ExpirationTimeStamp
            };
        }
}