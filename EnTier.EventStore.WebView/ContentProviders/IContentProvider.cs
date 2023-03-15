using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace EnTier.EventStore.WebView.ContentProviders
{
    interface IContentProvider
    {


        IContentProvider AppendChainAfter(IContentProvider next);
        
        Task Provide(HttpContext context);
    }
}