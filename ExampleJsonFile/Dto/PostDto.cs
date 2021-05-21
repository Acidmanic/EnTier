using System;

namespace ExampleJsonFile.Dto
{
    public class PostDto
    {
        public Guid Id { get; set; }
        
        public string Title { get; set; }
        
        public string Content { get; set; }
        
    }
}