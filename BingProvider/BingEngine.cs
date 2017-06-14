namespace BingProvider
{
    using Library;
    using Newtonsoft.Json;
    using Parliament.Search.OpenSearch;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Net.Http;
    using System.ServiceModel.Syndication;
    using System.Threading.Tasks;

    public class BingEngine : IEngine
    {
        public Feed Search(string searchTerms, int startIndex, int pageSize)
        {
            var serializer = new JsonSerializer();

            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", "edf33cb32e144f0a99ce8cbd610b44b3");
                using (var stream = client.GetStreamAsync($"https://api.cognitive.microsoft.com/bing/v5.0/search?q=site:parliament.uk+{searchTerms}").Result)
                {
                    using (var textReader = new StreamReader(stream))
                    {
                        using (var jsonReader = new JsonTextReader(textReader))
                        {
                            var result = serializer.Deserialize<BingResponse>(jsonReader);


                            var feed = new Feed
                            {
                                TotalResults = result.WebPages.TotalEstimatedMatches
                            };


                            feed.Items = result.WebPages.Values.Select(v =>
                            {
                                var item = new SyndicationItem
                                {
                                    Title = new TextSyndicationContent(v.Name),
                                    Summary = new TextSyndicationContent(v.Snippet, TextSyndicationContentKind.Html)
                                };

                                item.Links.Add(new SyndicationLink
                                {
                                    Uri = v.DisplayUrl,
                                    RelationshipType = "alternate"
                                });

                                return item;
                            });

                            return feed;
                        }
                    }
                }

            }
        }
    }

    public class BingResponse
    {
        [JsonProperty("webPages")]
        public WebPages WebPages { get; set; }
    }

    public class WebPages
    {
        [JsonProperty("totalEstimatedMatches")]
        public int TotalEstimatedMatches { get; set; }

        [JsonProperty("value")]
        public IList<Value> Values { get; set; }
    }

    public class Value
    {
        [JsonProperty("displayUrl")]
        public Uri DisplayUrl { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("snippet")]
        public string Snippet { get; set; }
    }
}
