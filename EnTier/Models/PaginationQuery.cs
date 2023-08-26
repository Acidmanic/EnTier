namespace EnTier.Models
{
    public class PaginationQuery
    {
        public int PageIndex { get; set; }
        
        public int PageSize { get; set; }
        
        public string SearchId { get; set; }

        public int Offset => (PageIndex -1) * PageSize;

        public int Size => PageSize;
    }
}