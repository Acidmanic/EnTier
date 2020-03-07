



using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StorageModels
{
    public class User
    {
        

        public string Name {get;set;}

        public string Surname {get;set;}

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id {get;set;}

        public List<Post> Posts{get;set;}
    }
}
