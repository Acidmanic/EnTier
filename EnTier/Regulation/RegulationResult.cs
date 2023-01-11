namespace EnTier.Regulation
{
    public class RegulationResult<TModel>
    {
        public RegulationStatus Status { get; set; }

        public TModel Model { get; set; }
        
    }
}