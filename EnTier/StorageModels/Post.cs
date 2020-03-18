


using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StorageModels{


    public class Post{

        public string Title{get;set;}

        public string Content{get;set;}

        [Key]
        public long Id {get;set;}
        public virtual User Creator {get;set;}

        public DateTime PostDate {get;set;}
    }
}