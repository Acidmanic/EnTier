using System;
using Acidmanic.Utilities.Results;
using EnTier.UnitOfWork;
using Microsoft.Extensions.Logging;

namespace EnTier.Utility
{
    public class DisposableFetchRepositoryResult<TCustom,TEntity,TId>: Result<TCustom>, IDisposable where TEntity : class, new()
    {
        private readonly IUnitOfWork _unitOfWork;

        public DisposableFetchRepositoryResult(IUnitOfWork unitOfWork, ILogger logger)
        {
            _unitOfWork = unitOfWork;

            var crudRepository = _unitOfWork.GetCrudRepository<TEntity, TId>();
                
            if (crudRepository is TCustom tRepository)
            {
                Value = tRepository;
                Success = true;
            }
            else
            {
                Value = default;
                Success = false;
                logger.LogError("You have forgotten to register an implementation of " +
                                "{TCustom} for ICrudRepository<{TE},{TI}>.",
                    typeof(TCustom).Name,typeof(TEntity).Name,typeof(TId).Name);
            }
        }

        public void Dispose()
        {
            if (Success)
            {
                _unitOfWork.Complete();
            }
        }
    }
}