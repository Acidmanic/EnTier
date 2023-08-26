using EnTier.DataAccess.Meadow.GenericFilteringRequests.Models;
using Meadow;
using Meadow.Requests;

namespace EnTier.DataAccess.Meadow.GenericFilteringRequests
{
    public sealed class RemoveExpiredFilterResultsRequest:MeadowRequest<ExpirationTimeStampShell,MeadowVoid>
    {
        public RemoveExpiredFilterResultsRequest(long expirationTimeStamp) : base(false)
        {
            ToStorage = new ExpirationTimeStampShell
            {
                ExpirationTimeStamp = expirationTimeStamp
            };
        }
    }
}