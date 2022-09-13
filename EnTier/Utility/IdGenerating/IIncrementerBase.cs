using System;
using Acidmanic.Utilities.Reflection;
using Acidmanic.Utilities.Reflection.TypeCenter;

namespace EnTier.Utility.IdGenerating
{


    public interface IIncrementer
    {
        
    }
    
    public interface IIncrementer<TId>
    {
        TId Increment(TId value);
    }
    
    public abstract class IncrementerBase<TId>:IIncrementer,IIncrementer<TId>
    {
        public abstract TId Increment(TId value);
        
    }
}