using System.Diagnostics;
using Acidmanic.Utilities.Reflection;
using Acidmanic.Utilities.Reflection.ObjectTree;
using EnTier.Prepopulation.Contracts;
using EnTier.Prepopulation.Models;
using EnTier.Utility;
using Microsoft.Extensions.Logging;

namespace EnTier.Prepopulation;

public class SeedPerformer<TStorage, TId> : ISeedingPerformer where TStorage : class, new()
{
    
    private SeedingToolBox ToolBox { get; }

    private SeedingProfile<TStorage> Profile { get; }
    private AccessNode IdLeaf { get; }

    private ILogger Logger => ToolBox.Logger;

    public SeedPerformer(SeedingProfile<TStorage> profile, SeedingToolBox toolBox)
    {
        Profile = profile;
        ToolBox = toolBox;
        IdLeaf = TypeIdentity.FindIdentityLeaf<TStorage, TId>();
    }


    public void Seed()
    {
        
        Logger.LogInformation("Performing Seeding for {Name}",Profile.Name);
        
        if (IdLeaf == null)
        {
            Logger.LogError("[{Name}]: Type {TypeName} has no available Id" +
                            " field which is mandatory for crud entities. Therefore seeding will be ABORTED.",
                Profile.Name, typeof(TStorage).FullName);
            return;
        }

        var repository = ToolBox.UnitOfWork.GetCrudRepository<TStorage, TId>();
        var anyMissing = false;

        foreach (var storage in Profile.SeedData)
        {
            Profile.Hook.PreIndexing(storage,ToolBox);
            
            var updated = repository.Set(storage);

            if (updated == null)
            {
                Logger.LogError("[{Name}]: Unable to seed object of type {TypeName}",
                    Profile.Name, typeof(TStorage).FullName);
                anyMissing = true;
            }
            else
            {
                var id = IdLeaf.Evaluator.Read(updated);

                IdLeaf.Evaluator.Write(storage, (TId)id);
            }
            
            Profile.Hook.PostIndexing(storage,ToolBox);
        }

        if (anyMissing)
        {
            Logger.LogWarning("[{Name}]: Seeding for entities of Type {TypeName} completed," +
                              " but not all objects has been seeded correctly.",
                Profile.Name, typeof(TStorage).FullName);
        }
    }

    public void Index()
    {
        if (!Profile.AlsoIndex)
        {
            return;
        }
        
        Logger.LogInformation("Performing Indexing for {Name}",Profile.Name);

        if (IdLeaf == null)
        {
            Logger.LogError("[{Name}]: Type {TypeName} has no available Id" +
                            " field which is mandatory for crud entities. Therefore seeding will be ABORTED.",
                Profile.Name, typeof(TStorage).FullName);
            return;
        }

        var repository = ToolBox.UnitOfWork.GetCrudRepository<TStorage, TId>();
        var anyMissing = false;
        foreach (var storage in Profile.SeedData)
        {
            Profile.Hook.PreIndexing(storage,ToolBox);
            
            var indexingObject = storage;

            var id = IdLeaf.Evaluator.Read(storage);
            
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
                    indexingObject = repository.GetById((TId)id, Profile.FullTreeIndexing);

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

                var indexed = repository.Index((TId)id, corpus);

                if (indexed == null)
                {
                    Logger.LogError("[{Name}]: There was a problem indexing object of type {TypeName}," +
                                    " with id {Id}.",Profile.Name,typeof(TStorage).FullName,id);
                    anyMissing = true;
                }
            }
            Profile.Hook.PostIndexing(storage,ToolBox);
        }

        if (anyMissing)
        {
            Logger.LogWarning("[{Name}]: Indexing for entities of Type {TypeName} completed," +
                              " but not all objects has been indexed correctly.",
                Profile.Name, typeof(TStorage).FullName);
        }
    }
}