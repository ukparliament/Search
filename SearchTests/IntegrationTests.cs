namespace SearchTests
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc.Testing;
    using Microsoft.Extensions.Configuration;
    using Microsoft.OpenApi.Readers;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Search;

    [TestClass]
    public class IntegrationTests
    {
        private static WebApplicationFactory<Startup> factory;
        private static HttpClient client;

        public static IEnumerable<object[]> ExtensionMappings => Configuration.Mappings.Select(m => new[] { m.Extension, m.MediaType });

        public static IEnumerable<object[]> MediaTypes => Configuration.Mappings.Select(m => new[] { m.MediaType });

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
        public async Task OpenApi_endpoint_works()
        {
            using (var response = await client.GetAsync("/openapi.json"))
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
