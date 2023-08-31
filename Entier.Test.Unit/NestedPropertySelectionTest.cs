using System.Linq;
using System.Threading;
using EnTier.Extensions;
using Xunit;

namespace Entier.Test.Unit
{
    public class NestedPropertySelectionTest
    {


        private class A
        {
            public string Name { get; set; }
            
            public B B { get; set; }
        }

        public class B
        {
            public string Description { get; set; }
            
            public C C { get; set; }
        }

        public class C
        {
            public string Title { get; set; }
        }
        

        [Fact]
        public void LambdaMustSelectNestedValues()
        {
            
            var expectedValue = "This is the value you should see";
            
            var obj = new A
            {
                Name = "Outer",
                B = new B { Description = "Inside the A", C = new C { Title = expectedValue } }
            };

            var collection = new A[] { obj };
            
            var standardAddress = "A.B.C.Title";

            var lambda = standardAddress.CreatePropertyPickerLambda<A, string>();

            var func = lambda.Compile();
            
            var chosen = collection.Select(func).FirstOrDefault();

            Assert.NotNull(chosen);
            
            Assert.Equal(expectedValue,chosen);

        }
    }
}