



using System.Collections.Generic;

namespace DataTransferModels
{
    

    public class User {

        public string Name{get;set;}

        public string Surname {get;set;}

        public long Id {get;set;}

        public List<Post> Posts{get;set;}

    }
}