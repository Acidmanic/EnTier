using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Acidmanic.Utilities.Reflection.Extensions;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace EnTier.EventStore.WebView
{
    public class OkEventsResult : IActionResult
    {
        private readonly IEnumerable<EventWrap> _events;

        public OkEventsResult(IEnumerable<EventWrap> events)
        {
            var evs = new List<EventWrap>(events as EventWrap[] ?? events.ToArray());

            evs.Sort(delegate(EventWrap a, EventWrap b)
                {
                    if (a.EventId is string sa && b.EventId is string sb)
                    {
                        return string.CompareOrdinal(sb, sa);
                    }

                    return (int)(b.EventId.AsNumber() - a.EventId.AsNumber());
                }
            );
            _events = evs;
        }

        public async Task ExecuteResultAsync(ActionContext context)
        {
            var wrapperObject = new { Events = _events };

            var response = context.HttpContext.Response;


            DefaultContractResolver contractResolver = new DefaultContractResolver
            {
                NamingStrategy = new CamelCaseNamingStrategy()
            };

            string json = JsonConvert.SerializeObject(wrapperObject, new JsonSerializerSettings
            {
                ContractResolver = contractResolver,
                Formatting = Formatting.Indented
            });

            var jsonData = Encoding.Default.GetBytes(json);

            await response.Body.WriteAsync(jsonData);
        }
    }
}