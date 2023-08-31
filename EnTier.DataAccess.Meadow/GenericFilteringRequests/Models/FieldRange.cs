using EnTier.Models;

namespace EnTier.DataAccess.Meadow.GenericFilteringRequests.Models
{
    public class FieldRange
    {
        public object Min { get; set; }
        
        public object Max { get; set; }


        public FilterRange ToFilterRange()
        {
            return new FilterRange
            {
                Maximum = Max?.ToString(),
                Minimum = Min?.ToString()
            };
        }
    }
}