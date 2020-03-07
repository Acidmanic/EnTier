




using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace DomainModels{



    public class Post{


        public string Title{get;set;}

        public string Content{get;set;}

        public long Id {get;set;}

        public long CreatorId {get;set;}

        public DateTime PostDate {get;set;}

    }
}