using System;
using EnTier.Prepopulation.Contracts;
using EnTier.Prepopulation.Models;

namespace EnTier.Prepopulation;

public abstract class SeedingHook<TStorage> : ISeedingHook<TStorage>
{
    private static readonly Action<TStorage, SeedingToolBox> DoNothing = (m, t) => { };


    private readonly Action<TStorage, SeedingToolBox> _preInsertion;
    private readonly Action<TStorage, SeedingToolBox> _postInsertion;
    private readonly Action<TStorage, SeedingToolBox> _preIndexing;
    private readonly Action<TStorage, SeedingToolBox> _postIndexing;

    protected SeedingHook(
        Action<TStorage, SeedingToolBox> postInsertion = null,
        Action<TStorage, SeedingToolBox> preInsertion = null,
        Action<TStorage, SeedingToolBox> preIndexing = null,
        Action<TStorage, SeedingToolBox> postIndexing = null)
    {
        _postInsertion = postInsertion ?? DoNothing;
        _preInsertion = preInsertion ?? DoNothing;
        _preIndexing = preIndexing ?? DoNothing;
        _postIndexing = postIndexing ?? DoNothing;
    }

    public SeedingHook(Action<TStorage, SeedingToolBox> postInsertion) : this(postInsertion, null, null, null)
    {
    }


    public virtual void PreInsertion(TStorage model, SeedingToolBox toolBox) => _preInsertion(model, toolBox);

    public virtual void PostInsertion(TStorage model, SeedingToolBox toolBox) => _postInsertion(model, toolBox);

    public virtual void PreIndexing(TStorage model, SeedingToolBox toolBox) => _preIndexing(model, toolBox);

    public virtual void PostIndexing(TStorage model, SeedingToolBox toolBox) => _postIndexing(model, toolBox);
}