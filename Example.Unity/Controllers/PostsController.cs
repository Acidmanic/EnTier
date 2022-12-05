using EnTier;
using EnTier.Controllers;
using Example.Unity.Contracts;
using Example.Unity.Models;
using Microsoft.AspNetCore.Mvc;

namespace Example.Unity.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class PostsController:CrudControllerBase<Post,long>
    {
        private readonly ITitleSuggestionService _titleSuggestionService;
        
        public PostsController(EnTierEssence essence, ITitleSuggestionService titleSuggestionService):base(essence)
        {
            _titleSuggestionService = titleSuggestionService;
        }
        
        public override IActionResult CreateNew(Post value)
        {
            if (string.IsNullOrEmpty(value.Title))
            {
                var newTitle = _titleSuggestionService.SuggestTitleFor(value.Content);
        
                value.Title = newTitle;
            }
            return base.CreateNew(value);
        }
        
    }
}