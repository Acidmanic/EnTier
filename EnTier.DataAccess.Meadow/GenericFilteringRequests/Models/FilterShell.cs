namespace EnTier.DataAccess.Meadow.GenericFilteringRequests.Models
{
    public class FilterShell
    {
        public string FilterHash { get; set; }
        
        public long ExpirationTimeStamp { get; set; }
        
        public string FilterExpression { get; set; }
    }
}
