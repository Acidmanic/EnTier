using EnTier;
using EnTier.Regulation;
using EnTier.Services;
using Example.Prepopulation.Models;

namespace Example.Prepopulation.Services;

public class ChocolateManipulatedService:CrudService<Chocolate,Chocolate,long,long>
{
    public ChocolateManipulatedService(EnTierEssence essence) : base(essence)
    {
    }

    public override Chocolate UpdateOrInsert(Chocolate value, bool alsoIndex, bool fullTreeIndexing)
    {
        value.FactoryName += "[Manipulated]";
        
        return base.UpdateOrInsert(value, alsoIndex, fullTreeIndexing);
    }
}