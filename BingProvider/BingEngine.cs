namespace BingProvider
{
    using Library;
    using Newtonsoft.Json;
    using Parliament.Search.OpenSearch;
    using System.Configuration;
    using System.IO;
    using System.Linq;
    using System.Net.Http;

    public class BingEngine : IEngine
    {
        public Feed Search(string searchTerms, int startIndex, int count)
        {
            var bingResponse = QueryBing(searchTerms, startIndex, count);
            var feed = ConvertToOpenSearchResponse(bingResponse, searchTerms, startIndex, count);

            return feed;
        }

        private static BingResponse QueryBing(string searchTerms, int startIndex, int count)
        {
            var serializer = new JsonSerializer();
            var query = new BingQuery
            {
                Site = "parliament.uk",
                QueryString = searchTerms,
                Offset = startIndex - 1,
                Count = count
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

        private static Feed ConvertToOpenSearchResponse(BingResponse bingResponse, string searchTerms, int startIndex, int count)
        {

            var openSearchResponse = new Response();

            var items = bingResponse.WebPages.Values.Select(value => openSearchResponse.ConvertToSyndicationItem(value.Name, value.Snippet, value.DisplayUrl.ToString(), value.Uri));

            return openSearchResponse.ConvertToOpenSearchResponse(items, bingResponse.WebPages.TotalEstimatedMatches, searchTerms, startIndex, count);
        }
    }
}
