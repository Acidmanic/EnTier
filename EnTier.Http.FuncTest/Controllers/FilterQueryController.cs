using System;
using System.Collections.Generic;
using System.Linq;
using EnTier.Extensions;
using EnTier.Http.FuncTest.Models;
using EnTier.Query;
using Microsoft.AspNetCore.Mvc;

namespace EnTier.Http.FuncTest.Controllers
{
    
    
    
    [ApiController]
    [Route("filter-queries")]
    public class FilterQueryController:ControllerBase
    {




        private class Shit
        {
            public class ShitItem
            {
                public string Name { get; set; }
                
                public EvaluationMethods Method { get; set; }
                
                public string Max { get; set; }
                
                public string Min { get; set; }
                
                public string[] Values { get; set; } = new string[]{};
                
                public string TypeName { get; set; }

                public ShitItem(FilterItem f)
                {
                    Name = f.Key;
                    Max = f.Maximum;
                    Min = f.Minimum;
                    Method = f.EvaluationMethod;
                    Values = f.EqualValues?.ToArray() ?? new string[] { };
                    TypeName = f.ValueType.FullName;
                }
            }
            public List<ShitItem> Filters { get; set; } 

            public string Hash { get; set; }
            public Shit( FilterQuery q)
            {
                Filters = q.Items().Select(i => new ShitItem(i)).ToList();

                Hash = q.Hash();
            }
        }
        

        [HttpGet]
        [Route("")]
        public IActionResult ReturnQuery()
        {
            var query = HttpContext.GetFilter<StorageModel>();

            var shit = new Shit(query);
            
            return Ok(shit);
        }
    }
}