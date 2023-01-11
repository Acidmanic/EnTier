using System.Collections.Generic;
using EnTier.Exceptions;
using EnTier.Utility;
using Xunit;

namespace Entier.Test.Unit
{
    public class DependencyResolverTests
    {



        [Fact]
        public void DependencyResolverMustBeAbleToSortValidDependencyMap()
        {
            var validMarkedMap = new Dictionary<string, List<string>>
            {
                {"A",new List<string>()},
                {"B",new List<string>{"A"}},
                {"C",new List<string>{"E","B"}},
                {"D",new List<string>{"G"}},
                {"E",new List<string>{"B"}},
                {"F",new List<string>{"C"}},
                {"G",new List<string>{"A"}}
            };

            var ordered = new string[] {"A","B","E","C","F","G","D" };

            var sut = new DependencyResolver<string>();

            var actual = sut.OrderByDependency(validMarkedMap);
            
            Assert.NotNull(actual);
            
            Assert.Equal(actual.Length,ordered.Length);

            for (int i = 0; i < ordered.Length; i++)
            {
                Assert.Equal(ordered[i],actual[i]);
            }
        }
        
        [Fact]
        public void DependencyResolverMustDetectLoopsAnThrowException()
        {
            var validMarkedMap = new Dictionary<string, List<string>>
            {
                {"A",new List<string>{"E"}},
                {"B",new List<string>{"A"}},
                {"C",new List<string>{"B"}},
                {"D",new List<string>{"C"}},
                {"E",new List<string>{"B"}},
                {"F",new List<string>{"C"}},
                {"G",new List<string>{"A"}}
            };
            
            var sut = new DependencyResolver<string>();

            Assert.Throws<DependencyLoopException>(() => sut.OrderByDependency(validMarkedMap));
        } 
    }
}