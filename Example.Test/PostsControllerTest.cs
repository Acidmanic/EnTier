using System;
using System.Collections.Generic;
using System.Net.Http;
using ExampleJsonFile.Storage;
using Microsoft.AspNetCore.Hosting;
using Newtonsoft.Json;
using Xunit;

namespace Example.UnitTest
{
    public class PostsControllerTest
    {
        [Fact]
        public void WithDbHavingPostWithDesiredTitle_PostShouldExistInResponse()
        {
            var builder = new WebHostBuilder();

            builder.UseKestrel(options => options.ListenLocalhost(9222));

            builder.UseStartup<TestStartup>();

            var app = builder.Build();

            app.Start();
            
            var client = new HttpClient();

            var result = client.GetStringAsync("http://localhost:9222/Posts").Result;

            var posts = JsonConvert.DeserializeObject<List<PostStg>>(result);
            
            Assert.NotEmpty(posts);
            
            Assert.Equal(2,posts.Count);
            
            Assert.Equal("DesiredTitle",posts[0].Title);
            
            app.Dispose();
        }
    }
}
