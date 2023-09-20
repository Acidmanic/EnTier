using EnTier.Prepopulation.Models;
using Microsoft.Extensions.Logging;

namespace EnTier.Prepopulation;

internal class ServiceLevelSeedPerformer<TDomain, TStorage, TDomainId, TStorageId> :
    SeedPerformerBase<TStorage,TDomain, TStorageId>
    where TStorage : class, new() where TDomain : class, new()
{
    
    public ServiceLevelSeedPerformer(SeedingProfile<TDomain> profile, SeedingToolBox toolBox) : base(profile, toolBox)
    {
    }

    protected override string LevelName => "Service";

    protected override bool PerformSeeding()
    {
        var service = ToolBox.MakeService<TDomain, TDomainId, TStorage, TStorageId>();

        var allGood = true;

        foreach (var domain in Profile.SeedData)
        {
            Profile.Hook.PreInsertion(domain,ToolBox);
            
            var updated = service.UpdateOrInsert(domain,false,false);
            
            if (updated == null)
            {
                Logger.LogError("[{Name}]: Unable to seed object of type {TypeName}",
                    Profile.Name, typeof(TStorage).FullName);
                allGood = false;
            }
            else
            {
                var id = SeedIdLeaf.Evaluator.Read(updated);

                SeedIdLeaf.Evaluator.Write(domain, (TDomainId)id);
            }
            
            Profile.Hook.PostInsertion(domain,ToolBox);
        }

        return allGood;
    }

    protected override bool PerformIndexing()
    {
        var service = ToolBox.MakeService<TDomain, TDomainId, TStorage, TStorageId>();

        var allGood = true;

        foreach (var domain in Profile.SeedData)
        {
            Profile.Hook.PreIndexing(domain,ToolBox);

            allGood &= service.TryIndex(domain).Result;
            
            Profile.Hook.PostIndexing(domain,ToolBox);
        }

        return allGood;
    }
}