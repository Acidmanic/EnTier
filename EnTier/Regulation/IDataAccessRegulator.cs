namespace EnTier.Regulation
{
    public interface IDataAccessRegulator<TDomain,TStorage>
    {
        RegulationResult<TDomain> RegulateIncoming(TDomain model);
        
        RegulationResult<TStorage> RegulateOutgoing(TStorage model);
        
    }
}