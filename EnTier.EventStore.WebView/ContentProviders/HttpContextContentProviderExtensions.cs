using System;
using Microsoft.AspNetCore.Http;

namespace EnTier.EventStore.WebView.ContentProviders
{
    internal static class HttpContextContentProviderExtensions
    {




        public static string RequestUri(this HttpContext context,string uriPrefix="")
        {
            var uri = context.Request.Path.ToString();

            if (uri.StartsWith("/"))
            {
                uri = uri.Substring(1, uri.Length - 1);
            }
            
            if (!string.IsNullOrEmpty(uriPrefix))
            {
                if (uri.ToLower().StartsWith(uriPrefix.ToLower()))
                {
                    uri = uri.Substring(uriPrefix.Length, uri.Length - uriPrefix.Length);
                }
            }

            return uri;
        }

        public static string[] RequestUriSegments(this HttpContext context, string uriPrefix = "")
        {
            var uri = context.RequestUri(uriPrefix);

            var segments = uri.Split(new[] { '/' }, StringSplitOptions.RemoveEmptyEntries);

            return segments;
        }
    }
}