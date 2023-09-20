using Acidmanic.Utilities.Reflection;
using Acidmanic.Utilities.Reflection.ObjectTree;
using EnTier.Prepopulation.Contracts;
using EnTier.Prepopulation.Interfaces;
using EnTier.Prepopulation.Models;
using Microsoft.Extensions.Logging;

namespace EnTier.Prepopulation;

internal abstract class SeedPerformerBase<TStorage,TSeedData, TId> : ISeedingPerformer where TStorage : class, new()
{
    
    protected SeedingToolBox ToolBox { get; }

    protected  SeedingProfile<TSeedData> Profile { get; }
    protected  AccessNode SeedIdLeaf { get; }

    protected ILogger Logger => ToolBox.Logger;

    
    protected abstract string LevelName { get; }
    
    public SeedPerformerBase(SeedingProfile<TSeedData> profile, SeedingToolBox toolBox)
    {
        Profile = profile;
        ToolBox = toolBox;
        SeedIdLeaf = TypeIdentity.FindIdentityLeaf<TSeedData, TId>();
    }


    public void Seed()
    {
        
        Logger.LogInformation("Performing Seeding for {Name} At {LevelName} Level",Profile.Name,LevelName);

        Profile.Initialize();
        
        if (SeedIdLeaf == null)
        {
            Logger.LogError("[{Name}]: Type {TypeName} has no available Id" +
                            " field which is mandatory for crud entities. Therefore seeding will be ABORTED.",
                Profile.Name, typeof(TStorage).FullName);
            return;
        }


        var anyMissing = !PerformSeeding();
        
        if (anyMissing)
        {
            Logger.LogWarning("[{Name}]: Seeding for entities of Type {TypeName} completed," +
                              " but not all objects has been seeded correctly.",
                Profile.Name, typeof(TStorage).FullName);
        }
    }


    protected abstract bool PerformSeeding();

    protected abstract bool PerformIndexing();
    
    public void Index()
    {
        if (!Profile.AlsoIndex)
        {
            return;
        }
        
        Logger.LogInformation("Performing Indexing for {Name} At {LevelName} Level",
            Profile.Name,LevelName);

        if (SeedIdLeaf == null)
        {
            Logger.LogError("[{Name}]: Type {TypeName} has no available Id" +
                            " field which is mandatory for crud entities. Therefore seeding will be ABORTED.",
                Profile.Name, typeof(TStorage).FullName);
            return;
        }

        var anyMissing = !PerformIndexing();

        if (anyMissing)
        {
            Logger.LogWarning("[{Name}]: Indexing for entities of Type {TypeName} completed," +
                              " but not all objects has been indexed correctly.",
                Profile.Name, typeof(TStorage).FullName);
        }
    }
}