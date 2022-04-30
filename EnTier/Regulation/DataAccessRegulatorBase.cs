namespace EnTier.Regulation
{
    public abstract class DataAccessRegulatorBase<TDomain,TStorage>:IDataAccessRegulator<TDomain,TStorage>
    {
        public abstract RegulationResult<TDomain, TStorage> Regulate(TDomain model);

        protected RegulationResult<TDomain, TStorage> Reject(TDomain model)
        {
            return new RegulationResult<TDomain, TStorage>
            {
                Model = model,
                Storage = default,
                Status = RegulationStatus.UnAcceptable
            };
        }

        protected RegulationResult<TDomain, TStorage> Accept(TDomain model, TStorage storage = default)
        {
            return new RegulationResult<TDomain, TStorage>
            {
                Model = model,
                Storage = storage,
                Status = RegulationStatus.Ok
            };
        }

        protected RegulationResult<TDomain, TStorage> ReportSuspicious(TDomain model, TStorage storage)
        {
            return new RegulationResult<TDomain, TStorage>
            {
                Model = model,
                Storage = storage,
                Status = RegulationStatus.Suspicious
            };
        }
    }
}