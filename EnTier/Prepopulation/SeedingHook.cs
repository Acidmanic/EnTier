using System;
using EnTier.Prepopulation.Contracts;
using EnTier.Prepopulation.Models;

namespace EnTier.Prepopulation;

public abstract class SeedingHook<TSeedData> : ISeedingHook<TSeedData>
{
    private static readonly Action<TSeedData, SeedingToolBox> DoNothing = (m, t) => { };


    private readonly Action<TSeedData, SeedingToolBox> _preInsertion;
    private readonly Action<TSeedData, SeedingToolBox> _postInsertion;
    private readonly Action<TSeedData, SeedingToolBox> _preIndexing;
    private readonly Action<TSeedData, SeedingToolBox> _postIndexing;

    protected SeedingHook(
        Action<TSeedData, SeedingToolBox> postInsertion = null,
        Action<TSeedData, SeedingToolBox> preInsertion = null,
        Action<TSeedData, SeedingToolBox> preIndexing = null,
        Action<TSeedData, SeedingToolBox> postIndexing = null)
    {
        _postInsertion = postInsertion ?? DoNothing;
        _preInsertion = preInsertion ?? DoNothing;
        _preIndexing = preIndexing ?? DoNothing;
        _postIndexing = postIndexing ?? DoNothing;
    }

    public SeedingHook(Action<TSeedData, SeedingToolBox> postInsertion) : this(postInsertion, null, null, null)
    {
    }


    public virtual void PreInsertion(TSeedData model, SeedingToolBox toolBox) => _preInsertion(model, toolBox);

    public virtual void PostInsertion(TSeedData model, SeedingToolBox toolBox) => _postInsertion(model, toolBox);

    public virtual void PreIndexing(TSeedData model, SeedingToolBox toolBox) => _preIndexing(model, toolBox);

    public virtual void PostIndexing(TSeedData model, SeedingToolBox toolBox) => _postIndexing(model, toolBox);
}