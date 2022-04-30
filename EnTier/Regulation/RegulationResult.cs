namespace EnTier.Regulation
{
    public class RegulationResult<TDomain,TStorage>
    {
        public RegulationStatus Status { get; set; }

        public TDomain Model { get; set; }
        
        public TStorage Storage { get; set; }
    }
}