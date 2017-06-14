namespace BingProvider
{
    using Library;
    using Newtonsoft.Json;
    using Parliament.Search.OpenSearch;
    using System.IO;
    using System.Linq;
    using System.Net.Http;
    using System.ServiceModel.Syndication;
    using System.Configuration;

    public class BingEngine : IEngine
    {
        public Feed Search(string searchTerms, int startIndex, int pageSize)
        {
            var bingResponse = BingEngine.QueryBing(searchTerms, startIndex, pageSize);
            var feed = BingEngine.ConvertToOpenSearch(bingResponse);

            return feed;
        }

        private static Feed ConvertToOpenSearch(BingResponse bingResponse)
        {
            return new Feed
            {
                TotalResults = bingResponse.WebPages.TotalEstimatedMatches,
                Items = bingResponse.WebPages.Values.Select(value =>
                {
                    var item = new SyndicationItem
                    {
                        Title = new TextSyndicationContent(value.Name),
                        Summary = new TextSyndicationContent(value.Snippet, TextSyndicationContentKind.Html)
                    };

                    item.Links.Add(new SyndicationLink
                    {
                        Title = value.DisplayUrl.ToString(),
                        Uri = value.Uri,
                        RelationshipType = "alternate"
                    });

                    return item;
                })
            };
        }

        private static BingResponse QueryBing(string searchTerms, int startIndex, int pageSize)
        {
            var serializer = new JsonSerializer();
            var query = new BingQuery
            {
                Site = "parliament.uk",
                QueryString = searchTerms,
                Offset = startIndex,
                Count = pageSize
            };

            var bingApiKey = ConfigurationManager.AppSettings["BingApiKey"];

            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", bingApiKey);
                using (var stream = client.GetStreamAsync(query).Result)
                {
                    using (var textReader = new StreamReader(stream))
                    {
                        using (var jsonReader = new JsonTextReader(textReader))
                        {
                            return serializer.Deserialize<BingResponse>(jsonReader);
                        }
                    }
                }
            }
        }
    }
}
