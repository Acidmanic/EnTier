using EnTier.DataAccess.Meadow.GenericFilteringRequests.Models;
using Meadow;
using Meadow.Extensions;
using Meadow.Requests;

namespace EnTier.DataAccess.Meadow.GenericFilteringRequests
{
    public sealed class RemoveExpiredFilterResultsRequest<TStorage>:MeadowRequest<ExpirationTimeStampShell,MeadowVoid>
    {
        public RemoveExpiredFilterResultsRequest(long expirationTimeStamp) : base(false)
        {
            ToStorage = new ExpirationTimeStampShell
            {
                ExpirationTimeStamp = expirationTimeStamp
            };
        }

        public override string RequestText
        {
            get => Configuration.GetNameConvention<TStorage>().RemoveExpiredFilterResultsProcedureName;
            protected set 
            {
                
            }
        }
    }
}