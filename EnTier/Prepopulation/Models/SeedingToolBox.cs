using EnTier.Contracts;
using EnTier.UnitOfWork;
using Microsoft.Extensions.Logging;

namespace EnTier.Prepopulation.Models;

public class SeedingToolBox
{
    internal SeedingToolBox(IUnitOfWork unitOfWork, ILogger logger, ITransliterationService transliterationService)
    {
        UnitOfWork = unitOfWork;
        Logger = logger;
        TransliterationService = transliterationService;
    }

    public IUnitOfWork UnitOfWork { get; }

    public ILogger Logger { get; }
    
    public ITransliterationService TransliterationService { get; }
    
    
}