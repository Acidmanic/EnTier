using System;
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
            var s = new Service();

            var info = s.CreateNew(new Info());

            System.Console.WriteLine(info.Id);
        }
    }
}
