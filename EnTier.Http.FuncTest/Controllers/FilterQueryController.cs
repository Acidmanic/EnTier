using System.Collections.Generic;
using System.Linq;
using Acidmanic.Utilities.Filtering;
using EnTier.Extensions;
using EnTier.Http.FuncTest.Models;
using Microsoft.AspNetCore.Mvc;

namespace EnTier.Http.FuncTest.Controllers
{
    
    
    
    [ApiController]
    [Route("filter-queries")]
    public class FilterQueryController:ControllerBase
    {




        private class FilterQueryDto
        {
            public class FilterItemDto
            {
                public string Name { get; set; }
                
                public ValueComparison Method { get; set; }
                
                public string Max { get; set; }
                
                public string Min { get; set; }
                
                public string[] Values { get; set; } = new string[]{};
                
                public string TypeName { get; set; }

                public FilterItemDto(FilterItem f)
                {
                    Name = f.Key;
                    Max = f.Maximum;
                    Min = f.Minimum;
                    Method = f.ValueComparison;
                    Values = f.EqualValues?.ToArray() ?? new string[] { };
                    TypeName = f.ValueType.FullName;
                }
            }
            public List<FilterItemDto> Filters { get; set; } 

            public string Hash { get; set; }
            
            public string FilterName { get; set; }
            
            
            public FilterQueryDto( FilterQuery q)
            {
                Filters = q.Items().Select(i => new FilterItemDto(i)).ToList();

                Hash = q.Hash();

                FilterName = q.FilterName;
            }
        }
        

        [HttpGet]
        [Route("")]
        public IActionResult ReturnQuery()
        {
            var query = HttpContext.GetFilter<StorageModel>();

            var queryDto = new FilterQueryDto(query);
            
            var fakeQuery = HttpContext.GetFilter<FakeStorageModel>();
            
            var fakeQueryDto = new FilterQueryDto(fakeQuery);
            
            return Ok(new
            {
                Original=queryDto,
                Fake=fakeQueryDto
            });
        }
    }
}