using System;
using System.Collections.Generic;
using Acidmanic.Utilities.Reflection;

namespace EnTier.TestByCase
{
    /// <summary>
    /// This class checks if CollectionDtoWrapper is working fine.
    /// </summary>
    public class Tbc002AutoWrapTest:TestBase
    {


        private class Model
        {
            public string Name { get; set; }
            
            public int Id { get; set; }
            
            public string Surname { get; set; }
        }
      
        public override void Main()
        {
            
            var data = new List<Model>
            {
                new Model
                {
                    Id = 1,
                    Name = "Mani",
                    Surname = "Moayedi"
                },
                new Model
                {
                    Id = 2,
                    Name = "J",
                    Surname = "Ashraf"
                }
            };
            
            var wrapped = new EnumerableDynamicWrapper<Model>().Wrap(data);

            Console.WriteLine(wrapped);
        }
    }
}