using System;
using EnTier.Bootstrap.Starters;
using EnTier.Context;
using EnTier.Service;
namespace Example3
{
    class Program
    {

        public class Info{
            public string Name {get;set;}

            public string Surname {get;set;}

            public long Id{get;set;}


        }

        public class Service:ServiceBase<Info,Info>{


        }
        static void Main(string[] args)
        {


            Scratch.Start(c => c.SetContext<FileContext>());

            var s = new Service();


            var all = s.GetAll();

            all.ForEach(m => Print(m));

            var info = s.CreateNew(new Info() { Name = "Mani Moayedi", Surname = DateTime.Now.ToShortDateString()});


            Console.WriteLine("Added new one:");

            Print(info);

            Console.WriteLine("Press any key to exit");

            Console.ReadKey();
        }

        private static void Print(Info m)
        {
            Console.WriteLine(m.Id + "-\t" + m.Name + " " + m.Surname);
        }
    }
}
