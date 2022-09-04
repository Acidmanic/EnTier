using System;
using System.Collections.Generic;
using EnTier.Results;
using EnTier.UnitOfWork;

namespace EnTier.Prepopulation
{
    public abstract class PrepopulationSeedBase<TModel, TId> : PrepopulationSeedBase where TModel : class, new()
    {
        protected readonly IUnitOfWork UnitOfWork;

        protected readonly SeedingContext<TModel, TId> SeedingContext;

        public PrepopulationSeedBase(IUnitOfWork unitOfWork)
        {
            UnitOfWork = unitOfWork;

            SeedingContext = new SeedingContext<TModel, TId>(unitOfWork);
        }

        protected Result SeedAll(IEnumerable<TModel> seeds)
        {
            return SeedAll(seeds, s => { });
        }

        protected Result SeedAll(IEnumerable<TModel> seeds, Action<TModel> onInsertion)
        {
            return SeedAll<TModel, TId>(seeds, onInsertion, SeedingContext);
        }

        protected Result SeedAll<TM, TI>(IEnumerable<TM> seeds, Action<TM> onInsertion) where TM : class, new()
        {
            var context = new SeedingContext<TM, TI>(UnitOfWork);

            return SeedAll(seeds, onInsertion, context);
        }

        private Result SeedAll<TM, TI>(IEnumerable<TM> seeds, Action<TM> onInsertion, SeedingContext<TM, TI> context)
            where TM : class, new()
        {
            foreach (var seed in seeds)
            {
                if (SeedOne(seed, context))
                {
                    onInsertion(seed);
                }
            }

            UnitOfWork.Complete();

            return new Result().Succeed();
        }

        private Result SeedOne<TM, TI>(TM model, SeedingContext<TM, TI> context) where TM : class, new()
        {
            var updatedEntity = context.Repository.Set(model);

            if (updatedEntity == null)
            {
                return new Result().Fail();
            }

            if (SeedingContext.HasId)
            {
                var id = context.IdLeaf.Evaluator.Read(updatedEntity);

                context.IdLeaf.Evaluator.Write(model, id);
            }

            return new Result().Succeed();
        }

        protected Result SeedOne<TM, TI>(TM model) where TM : class, new()
        {
            var context = new SeedingContext<TM, TI>(UnitOfWork);

            var result = SeedOne(model, context);
            
            UnitOfWork.Complete();

            return result;
        }
    }
}