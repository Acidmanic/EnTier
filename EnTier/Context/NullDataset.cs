


using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace EnTier.Context
{


    public class NullDataset<T> : IDataset<T>,IEnTierBuiltIn
    where T:class
    {
        public T Add(T item)
        {
            LogFunctionCall();
            return item;
        }

        private void LogFunctionCall()
        {
            System.Console.WriteLine("{0} has been called on {1} class."
            ,MethodBase.GetCurrentMethod().Name,this.GetType().Name);
        }

        public IQueryable<T> AsQueryable()
        {
            LogFunctionCall();

            return new List<T>().AsQueryable();
        }

        public T Remove(T item)
        {
            LogFunctionCall();

            return item;
        }
    }
}