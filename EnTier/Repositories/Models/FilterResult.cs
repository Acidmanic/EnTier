using Acidmanic.Utilities.Reflection.Attributes;

namespace EnTier.Repositories.Models
{
    public class FilterResult
    {
        
        /// <summary>
        /// This field should be set by filter-query's hash 
        /// </summary>
        [UniqueMember]
        public string Id { get; set; }
        
        public long ResultId { get; set; }
        
        /// <summary>
        /// This field should be set when the filter is performed
        /// </summary>
        public long ExpirationTimeStamp { get; set; }
        

    }
}