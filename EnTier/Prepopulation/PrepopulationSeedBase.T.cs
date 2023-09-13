// using System;
// using System.Collections.Generic;
// using System.Diagnostics.CodeAnalysis;
// using Acidmanic.Utilities.Results;
// using EnTier.Contracts;
// using EnTier.Services.Transliteration;
// using EnTier.UnitOfWork;
// using EnTier.Utility;
//
// namespace EnTier.Prepopulation
// {
//     public abstract class PrepopulationSeedBase<TModel, TId> : PrepopulationSeedBase where TModel : class, new()
//     {
//         //protected readonly IUnitOfWork UnitOfWork;
//
//         protected readonly SeedingContext<TModel, TId> SeedingContext;
//
//         /// <summary>
//         /// This constructor, constructs a prepopulation seed that tries to index entries after insertion.
//         /// You need to provide 'alsoIndex' and 'fullTreeIndexing' parameters with your choices,
//         /// in your implementations. Leave no un-injectable parameters in constructor
//         /// </summary>
//         /// <param name="unitOfWork">Just pass-forward to base constructor, if your EnTier is correctly
//         /// configured, it would be provided.
//         /// </param>
//         /// <param name="transliterationService">You can pass a null and use builtin transliteration service
//         /// , or you can register your implementation in your DI.
//         /// </param>
//         public PrepopulationSeedBase(IUnitOfWork unitOfWork, [AllowNull] ITransliterationService transliterationService)
//         {
//             transliterationService ??= new EnTierBuiltinTransliterationsService();
//
//             SeedingContext =
//                 new SeedingContext<TModel, TId>(unitOfWork,
//                     transliterationService);
//         }
//
//         /// <summary>
//         /// This constructor constructs a prepopulation seed that would NOT try to index entries. 
//         /// </summary>
//         /// <param name="unitOfWork">
//         /// Just pass-forward to base constructor, if your EnTier is correctly
//         /// configured, it would be provided.
//         /// </param>
//         public PrepopulationSeedBase(IUnitOfWork unitOfWork) : this(unitOfWork, new EnTierBuiltinTransliterationsService())
//         {
//         }
//
//         protected Result SeedAll(IEnumerable<TModel> seeds)
//         {
//             return SeedAll(seeds, s => { });
//         }
//
//         protected Result SeedAll(IEnumerable<TModel> seeds, Action<TModel> onInsertion)
//         {
//             return SeedAll<TModel, TId>(seeds, onInsertion, SeedingContext);
//         }
//
//
//         private SeedingContext<TM, TI> CreateSeedingContext<TM, TI>()
//             where TM : class, new()
//         {
//             return new SeedingContext<TM, TI>(
//                 SeedingContext.UnitOfWork,
//                 SeedingContext.TransliterationService);
//         }
//
//         protected Result SeedAll<TM, TI>(IEnumerable<TM> seeds, Action<TM> onInsertion) where TM : class, new()
//         {
//             var context = CreateSeedingContext<TM, TI>();
//
//             return SeedAll(seeds, onInsertion, context);
//         }
//
//         private Result SeedAll<TM, TI>(IEnumerable<TM> seeds, Action<TM> onInsertion, SeedingContext<TM, TI> context)
//             where TM : class, new()
//         {
//             foreach (var seed in seeds)
//             {
//                 if (SeedOne(seed, context))
//                 {
//                     onInsertion(seed);
//                 }
//             }
//
//             context.UnitOfWork.Complete();
//
//             return new Result().Succeed();
//         }
//
//         private Result SeedOne<TM, TI>(TM model, SeedingContext<TM, TI> context) where TM : class, new()
//         {
//             var updatedEntity = context.Repository.Set(model);
//
//             if (updatedEntity == null)
//             {
//                 return new Result().Fail();
//             }
//
//             if (context.HasId)
//             {
//                 var id = context.IdLeaf.Evaluator.Read(updatedEntity);
//
//                 context.IdLeaf.Evaluator.Write(model, id);
//
//                 if (context.AlsoIndex)
//                 {
//                     var indexingObject = updatedEntity;
//                     
//                     if (context.FullTreeIndexing)
//                     {
//                         indexingObject = context.Repository.GetById((TI)id, true);
//                     }
//                     
//                     var rawCorpus = ObjectUtilities.ExtractAllTexts(indexingObject, context.FullTreeIndexing);
//
//                     var indexCorpus = context.TransliterationService.Transliterate(rawCorpus);
//
//                     var indexed = context.Repository.Index((TI)id, indexCorpus);
//
//                     if (indexed == null)
//                     {
//                         return new Result().Fail();
//                     }
//                 }
//             }
//
//             return new Result().Succeed();
//         }
//
//         protected Result SeedOne<TM, TI>(TM model, bool alsoIndex = false, bool fullTreeIndexing = false)
//             where TM : class, new()
//         {
//             var context = CreateSeedingContext<TM, TI>(alsoIndex, fullTreeIndexing);
//
//             var result = SeedOne(model, context);
//
//             context.UnitOfWork.Complete();
//
//             return result;
//         }
//     }
// }