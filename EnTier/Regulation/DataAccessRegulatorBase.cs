namespace EnTier.Regulation
{
    public abstract class DataAccessRegulatorBase<TDomain,TStorage>:IDataAccessRegulator<TDomain,TStorage>
    {
        

        protected RegulationResult<TModel> Reject<TModel>()
        {
            return new RegulationResult<TModel>
            {
                Model = default,
                Status = RegulationStatus.UnAcceptable
            };
        }

        protected RegulationResult<TModel> Accept<TModel>(TModel model)
        {
            return new RegulationResult<TModel>
            {
                Model = model,
                Status = RegulationStatus.Ok
            };
        }

        protected RegulationResult<TModel> Suspect<TModel>(TModel model)
        {
            return new RegulationResult<TModel>
            {
                Model = model,
                Status = RegulationStatus.Suspicious
            };
        }

        public abstract RegulationResult<TDomain> RegulateIncoming(TDomain model);
        public abstract RegulationResult<TStorage> RegulateOutgoing(TStorage model);
    }
}