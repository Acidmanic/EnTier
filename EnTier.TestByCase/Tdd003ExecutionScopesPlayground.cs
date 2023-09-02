using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Emit;
using System.Threading.Tasks;
using Acidmanic.Utilities.Reflection.Attributes;
using Microsoft.AspNetCore.Authorization;

namespace EnTier.TestByCase;

public class Tdd003ExecutionScopesPlayground : TestBase
{
    private class StackInfo
    {
        public static List<Attribute> ListDeliveredAttributes()
        {
            var list = new List<Attribute>();

            var stack = new StackTrace();

            var frames = stack.GetFrames() ?? new StackFrame[] { };

            foreach (var frame in frames)
            {
                var myAttributes = frame?.GetMethod()?.GetCustomAttributes(true);

                foreach (var attribute in myAttributes)
                {
                    if (attribute is Attribute a)
                    {
                        list.Add(a);
                    }
                }
            }

            return list;
        }
    }


    [AlteredType(typeof(int))]
    private class Rat
    {
        public void Report()
        {
            var myAttributes = StackInfo.ListDeliveredAttributes();

            foreach (var attribute in myAttributes)
            {
                Console.WriteLine($"Attribute: {attribute.GetType().FullName}");
            }
        }
    }

    private class AsyncRat : Rat
    {
       
        public Task ReportAsync()
        {

            var myAttributes = StackInfo.ListDeliveredAttributes();

            
            foreach (var attribute in myAttributes)
            {
            
            }
            
            
            
            return Task.Run(Report);
        }
    }


    [Authorize]
    public override void Main()
    {
        var r = new AsyncRat();

        r.ReportAsync().Wait();
    }
}