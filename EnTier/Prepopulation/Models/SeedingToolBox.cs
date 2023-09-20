using System;
using System.Diagnostics;
using EnTier.Contracts;
using EnTier.Mapper;
using EnTier.Services;
using EnTier.Services.Transliteration;
using EnTier.UnitOfWork;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.LightWeight;

namespace EnTier.Prepopulation.Models;

public class SeedingToolBox
{
    private readonly EnTierEssence _essence;
    internal SeedingToolBox(EnTierEssence essence)
    {
        _essence = essence;
        
        Logger = essence.Logger ?? new ConsoleLogger();
        
        UnitOfWork = essence.UnitOfWork;
        
        TransliterationService = essence.TransliterationService  ?? new EnTierBuiltinTransliterationsService();

        Mapper = essence.Mapper;
    }

    public IUnitOfWork UnitOfWork { get; }

    public ILogger Logger { get; }
    
    public ITransliterationService TransliterationService { get; }
    
    public IMapper Mapper { get; }

    public ICrudService<TDomain, TDomainId> MakeService<TDomain, TDomainId, TStorage, TStorageId>() 
        where TDomain : class, new() 
        where TStorage : class, new()
    {
        return _essence.ResolveOrDefault<ICrudService<TDomain, TDomainId>>(
            new CrudService<TDomain,TStorage,TDomainId,TStorageId>(_essence));
    } 
}