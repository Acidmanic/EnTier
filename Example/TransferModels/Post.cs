




using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataTransferModels{



    public class Post{


        public string Title{get;set;}

        public string Content{get;set;}

        public long Id {get;set;}

        public User Creator {get;set;}
        public DateTime PostDate {get;set;}

    }
}