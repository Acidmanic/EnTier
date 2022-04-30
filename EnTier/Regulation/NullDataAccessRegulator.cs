namespace EnTier.Regulation
{
    public abstract class NullDataAccessRegulator
    {
    }

    public class NullDataAccessRegulator<TDomain, TStorage> : NullDataAccessRegulator, IDataAccessRegulator<TDomain, TStorage>
    {
        public RegulationResult<TDomain, TStorage> Regulate(TDomain model)
        {
            return new RegulationResult<TDomain, TStorage>
            {
                Model = model,
                Status = RegulationStatus.Ok,
                Storage = default
            };
        }
    }
}