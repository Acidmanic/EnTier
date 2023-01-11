namespace EnTier.Regulation
{
    public abstract class NullDataAccessRegulator
    {
    }

    public class NullDataAccessRegulator<TDomain, TStorage> : NullDataAccessRegulator, IDataAccessRegulator<TDomain, TStorage>
    {
        public RegulationResult<TDomain> RegulateIncoming(TDomain model)
        {
            return new RegulationResult<TDomain>
            {
                Model = model,
                Status = RegulationStatus.Ok
            };
        }

        public RegulationResult<TStorage> RegulateOutgoing(TStorage model)
        {
            return new RegulationResult<TStorage>
            {
                Model = model,
                Status = RegulationStatus.Ok
            };
        }
    }
}