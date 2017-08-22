namespace BingProvider
{
    using Library;
    using Newtonsoft.Json;
    using Parliament.Search.OpenSearch;
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.IO;
    using System.Linq;
    using System.Net.Http;
    using System.ServiceModel.Syndication;

    public class BingEngine : IEngine
    {
        public Feed Search(string searchTerms, int startIndex, int count)
        {
            var openSearchResponse = new Response();

            var items = new List<SyndicationItem>();

            var BingMaxResultsPerQuery = 50;

            var result = QueryBing(searchTerms, startIndex, Math.Min(BingMaxResultsPerQuery, count));

            int totalResults = 0;

            if (result.WebPages != null)
            {
                totalResults = result.WebPages.TotalEstimatedMatches;

                items.AddRange(result.WebPages.Values.Select(item => openSearchResponse.ConvertToSyndicationItem(item.Name, item.Snippet, item.DisplayUrl.ToString(), item.Uri)));

                for (int nextIndex = startIndex + items.Count(); nextIndex <= totalResults && items.Count() < count; nextIndex += BingMaxResultsPerQuery)
                {
                    var nextCount = Math.Min(BingMaxResultsPerQuery, count - items.Count());

                    var nextResult = QueryBing(searchTerms, nextIndex, nextCount);

                    if (nextResult.WebPages.Values.Count() == 0)
                    {
                        break;
                    }
                    items.AddRange(nextResult.WebPages.Values.Select(item => openSearchResponse.ConvertToSyndicationItem(item.Name, item.Snippet, item.DisplayUrl.ToString(), item.Uri)));

                }
            }

            return openSearchResponse.ConvertToOpenSearchResponse(items, totalResults, searchTerms, startIndex, count);
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
    }
}
