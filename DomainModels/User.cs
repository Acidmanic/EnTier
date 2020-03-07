



using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DomainModels
{

    public class User{

        public string Name{get;set;}

        public string Surname {get;set;}

        public long Id {get;set;}

        public List<Post> Posts {get;set;}
        
    }
    
}