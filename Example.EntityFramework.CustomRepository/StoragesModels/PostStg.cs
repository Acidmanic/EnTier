using System.ComponentModel.DataAnnotations;
using System.Data.Common;

namespace ExampleEntityFramework.StoragesModels
{
    public class PostStg
    {
        
        public long PostStgId { get; set; }
        
        public long Id { get; set; }
        
        public string Title { get; set; }
        
        public string Content { get; set; }
        
    }
}