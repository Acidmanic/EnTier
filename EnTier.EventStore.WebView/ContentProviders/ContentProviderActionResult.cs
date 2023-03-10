using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace EnTier.EventStore.WebView.ContentProviders
{
    internal class ContentProviderActionResult : IActionResult
    {
        private readonly IContentProvider _contentProvider;

        public ContentProviderActionResult(IContentProvider contentProvider)
        {
            _contentProvider = contentProvider;
        }

        public Task ExecuteResultAsync(ActionContext context)
        {
            return Task.Run(() => { _contentProvider.Provide(context.HttpContext); });
        }
    }
}