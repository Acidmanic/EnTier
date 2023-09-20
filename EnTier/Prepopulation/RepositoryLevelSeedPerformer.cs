using System.Diagnostics;
using EnTier.Prepopulation.Models;
using EnTier.Utility;
using Microsoft.Extensions.Logging;

namespace EnTier.Prepopulation;

internal class RepositoryLevelSeedPerformer<TStorage,TStorageId>:SeedPerformerBase<TStorage,TStorage,TStorageId> 
    where TStorage : class, new()
{
    public RepositoryLevelSeedPerformer(SeedingProfile<TStorage> profile, SeedingToolBox toolBox) : base(profile, toolBox)
    {
    }

    protected override string LevelName => "Repository";
    protected override bool PerformSeeding()
    {
        var repository = ToolBox.UnitOfWork.GetCrudRepository<TStorage, TStorageId>();
        var anyMissing = false;

        foreach (var storage in Profile.SeedData)
        {
            Profile.Hook.PreInsertion(storage,ToolBox);
            
            var updated = repository.Set(storage);

            if (updated == null)
            {
                Logger.LogError("[{Name}]: Unable to seed object of type {TypeName}",
                    Profile.Name, typeof(TStorage).FullName);
                anyMissing = true;
            }
            else
            {
                var id = SeedIdLeaf.Evaluator.Read(updated);

                SeedIdLeaf.Evaluator.Write(storage, (TStorageId)id);
            }
            
            Profile.Hook.PostInsertion(storage,ToolBox);
        }

        return !anyMissing;
        
    }

    protected override bool PerformIndexing()
    {
        var repository = ToolBox.UnitOfWork.GetCrudRepository<TStorage, TStorageId>();
        var anyMissing = false;
        
        foreach (var storage in Profile.SeedData)
        {
            Profile.Hook.PreIndexing(storage,ToolBox);
            
            var indexingObject = storage;

            var id = SeedIdLeaf.Evaluator.Read(storage);
            
            if (id == null)
            {
                Logger.LogError("[{Name}]: Object of type {TypeName}, has not been seeded correctly, " +
                                "so it was omitted from indexing.", Profile.Name,
                    typeof(TraceSource).FullName);
            }
            else
            {
                if (Profile.FullTreeIndexing)
                {
                    indexingObject = repository.GetById((TStorageId)id, Profile.FullTreeIndexing);

                    if (indexingObject == null)
                    {
                        Logger.LogWarning("[{Name}]: Object of type {TypeName}, with Id: {Id}," +
                                          "could not be read full tree for indexing, so plain-object " +
                                          "is being used instead.",Profile.Name,
                            typeof(TStorage).FullName,id);
                        indexingObject = storage;
                    }
                }

                var rawCorpus = ObjectUtilities.ExtractAllTexts(indexingObject, Profile.FullTreeIndexing);
                
                var corpus = ToolBox.TransliterationService.Transliterate(rawCorpus);

                var indexed = repository.Index((TStorageId)id, corpus);

                if (indexed == null)
                {
                    Logger.LogError("[{Name}]: There was a problem indexing object of type {TypeName}," +
                                    " with id {Id}.",Profile.Name,typeof(TStorage).FullName,id);
                    anyMissing = true;
                }
            }
            Profile.Hook.PostIndexing(storage,ToolBox);
        }

        return !anyMissing;
    }
}