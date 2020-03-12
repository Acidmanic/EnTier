


using System.ComponentModel.DataAnnotations;

namespace StorageModels {



    public class Project{


        public string Name{get;set;}

        public string Description {get;set;}

        public string Repository {get;set;}

        [Key]
        public long Id {get;set;}
    }
}