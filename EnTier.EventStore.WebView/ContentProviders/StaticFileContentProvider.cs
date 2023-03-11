using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Acidmanic.Utilities.Reflection.Extensions;
using Acidmanic.Utilities.SourceResource;
using Microsoft.AspNetCore.Http;

namespace EnTier.EventStore.WebView.ContentProviders
{
    internal class StaticFileContentProvider : IContentProvider
    {
        private static object _locker = new object();
        private static bool _filesExtracted = false;
        
        
        private readonly string _contentRootDirectory;
        private readonly string _uriPrefix;
        private IContentProvider _next = null;

        private List<string> DefaultDocuments { get; } = new List<string>();

        public StaticFileContentProvider(string contentRootDirectory, string uriPrefix = "")
        {
            _contentRootDirectory = contentRootDirectory;
            _uriPrefix = uriPrefix;
            CheckExtraction();
        }


        private void CheckExtraction()
        {
            lock (_locker)
            {
                if (!_filesExtracted)
                {
                    _filesExtracted = true;
                    
                    new SourceDataBuilder().ExtractIntoDirectory(_contentRootDirectory,GetType().Assembly);
                }
            }
        }

        public IContentProvider AppendChainAfter(IContentProvider next)
        {
            _next = next;

            return this;
        }

        public StaticFileContentProvider AddDefaultDocument(string documentName)
        {
            DefaultDocuments.Add(documentName);

            return this;
        }

        public async Task Provide(HttpContext context)
        {
            var uriSegments = context.RequestUriSegments(_uriPrefix);

            var contentRootDirectory = new DirectoryInfo(_contentRootDirectory).FullName;

            var path = contentRootDirectory + Path.DirectorySeparatorChar + string.Join(Path.DirectorySeparatorChar, uriSegments);
            
            if (!File.Exists(path))
            {
                foreach (var defaultDocument in DefaultDocuments)
                {
                    path = Path.Combine(contentRootDirectory, defaultDocument);

                    if (File.Exists(path))
                    {
                        break;
                    }
                }
            }
            
            
            if (File.Exists(path))
            {
                var content = await File.ReadAllBytesAsync(path);

                var mime = MediaTypes.GetContentTypeForFile(path);
                
                context.Response.Headers.Clear();

                context.Response.Headers.Add("X-Content-Type-Options", "nosniff");
                context.Response.Headers.Add("Cache-Control", "no-cache, no-store, must-revalidate");
                context.Response.Headers.Add("Pragma", "no-cache");
                context.Response.Headers.Add("Content-Type", mime);

                context.Response.ContentType = mime;

                await context.Response.Body.WriteAsync(content);
                
                await context.Response.Body.FlushAsync();

                await context.Response.Body.DisposeAsync();

                return;
            }

            if (_next != null)
            {
                await _next.Provide(context);
            }
        }
    }
}