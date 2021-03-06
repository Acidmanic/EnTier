using Microsoft.AspNetCore.Mvc;

namespace EnTier.Controllers
{

    public class SafeRunResult<TReturn>{

        public TReturn Result{get; set;}
        
        public IActionResult ErrorReturningResult{get; set;}

        public bool Success {get;set;}
        
    }
  
}