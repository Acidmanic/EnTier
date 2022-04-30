namespace EnTier.Regulation
{
    public interface IDataAccessRegulator<TDomain,TStorage>
    {
        RegulationResult<TDomain,TStorage> Regulate(TDomain model);
        
    }
}