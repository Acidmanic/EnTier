using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace EnTier.EventStore.WebView.ContentProviders
{
    internal class InMemoryIndexPageContentProvider:IContentProvider
    {
        public IContentProvider AppendChainAfter(IContentProvider next)
        {
            return this;
        }

        public async Task Provide(HttpContext context)
        {
            context.Response.ContentType = "text/html";

            var content = "<H1>Not found</H1>";

            var contentData = Encoding.Default.GetBytes(content);
            
            await context.Response.Body.WriteAsync(contentData);
        }
    }
}