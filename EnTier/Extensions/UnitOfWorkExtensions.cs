using System.Collections.Generic;
using Acidmanic.Utilities.Reflection;
using EnTier.Services.Transliteration;
using EnTier.UnitOfWork;
using EnTier.Utility;

namespace EnTier.Extensions;

public static class UnitOfWorkExtensions
{
    public static void UpdateIndexes<TStorage, TId>(this IUnitOfWork unitOfWork, bool fullTree)
        where TStorage : class, new()
    {
        var idLeaf = TypeIdentity.FindIdentityLeaf<TStorage>();

        if (idLeaf != null)
        {
            var repository = unitOfWork.GetCrudRepository<TStorage, TId>();

            if (repository != null)
            {
                IEnumerable<TStorage> allData = repository.All(fullTree);

                var transliteration = new EnTierBuiltinTransliterationsService();

                foreach (var storage in allData)
                {
                    var rawCorpus = ObjectUtilities.ExtractAllTexts(storage, fullTree);

                    var indexCorpus = transliteration.Transliterate(rawCorpus);

                    var id = (TId)idLeaf.Evaluator.Read(storage);

                    repository.Index(id, indexCorpus);
                }
            }
        }
    }
}