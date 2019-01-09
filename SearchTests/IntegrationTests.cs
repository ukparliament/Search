// MIT License
//
// Copyright (c) 2019 UK Parliament
//
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

namespace SearchTests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc.Testing;
    using Microsoft.Extensions.Configuration;
    using Microsoft.OpenApi;
    using Microsoft.OpenApi.Readers;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Search;

    [TestClass]
    public class IntegrationTests
    {
        private static WebApplicationFactory<Startup> factory;
        private static HttpClient client;

        public static IEnumerable<object[]> ExtensionMappings => Configuration.QueryMappings.Select(m => new[] { m.Extension, m.MediaType });

        public static IEnumerable<object[]> MediaTypes => Configuration.QueryMappings.Select(m => new[] { m.MediaType });

        public static IEnumerable<object[]> OpenApiExtensions
        {
            get
            {
                var noExtension = new[] { string.Empty };
                var allowedExtensions = Enum.GetNames(typeof(OpenApiFormat)).Select(name => $".{name.ToLower()}");

                return noExtension.Union(allowedExtensions).Select(e => (new[] { e }));
            }
        }

        [ClassInitialize]
        public static void Initialize(TestContext context)
        {
            factory = new WebApplicationFactory<Startup>()
                .WithWebHostBuilder(
                    hostBuilder => hostBuilder.ConfigureAppConfiguration(
                        (builderContext, configurationBuiolder) => configurationBuiolder.AddInMemoryCollection(
                            new Dictionary<string, string> {
                                { "Engine", "Search.MockEngine" }})));

            client = factory.CreateClient();
        }

        [ClassCleanup]
        public static void Cleanup()
        {
            client.Dispose();
            factory.Dispose();
        }

        [TestMethod]
        [DynamicData(nameof(OpenApiExtensions))]
        public async Task OpenApi_endpoint_works(string extension)
        {
            using (var response = await client.GetAsync($"/openapi{extension}"))
            {
                var reader = new OpenApiStreamReader();

                using (var stream = await response.Content.ReadAsStreamAsync())
                {
                    reader.Read(stream, out var diagnostic);

                    Assert.IsFalse(diagnostic.Errors.Any(), string.Join(",", diagnostic.Errors));
                }
            }
        }

        [TestMethod]
        public async Task SwaggerUI_works()
        {
            using (var response = await client.GetAsync("/"))
            {
                var result = await response.Content.ReadAsStringAsync();

                StringAssert.Contains(result, "SwaggerUIBundle");
            }
        }

        [TestMethod]
        [DynamicData(nameof(ExtensionMappings))]
        public async Task Negotiates_extensions(string extension, string mediaType)
        {
            using (var response = await client.GetAsync($"/query.{extension}"))
            {
                Assert.AreEqual(mediaType, response.Content.Headers.ContentType.MediaType);
            }
        }

        [TestMethod]
        [DynamicData(nameof(MediaTypes))]
        public async Task Negotiates_accept_headers(string mediaType)
        {
            using (var request = new HttpRequestMessage(HttpMethod.Get, "/query"))
            {
                request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue(mediaType));

                using (var response = await client.SendAsync(request))
                {
                    Assert.AreEqual(mediaType, response.Content.Headers.ContentType.MediaType);
                }
            }
        }
    }
}
