namespace BingProvider
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Linq;
    using System.Threading.Tasks;
    using Library;
    using Microsoft.Azure.CognitiveServices.Search.WebSearch;
    using Parliament.Search.OpenSearch;
    using Bing = Microsoft.Azure.CognitiveServices.Search.WebSearch.Models;

    public class BingEngine : IEngine
    {
        public async Task<Feed> Query(string searchTerms, int startIndex, int count)
        {
            if (count > 50)
            {
                throw new ArgumentOutOfRangeException(nameof(count), count, "max 50");
            }

            var result = await QueryBing(searchTerms, startIndex, count);

            if (result == null)
            {
                return Response.ConvertToOpenSearchResponse(null, 0, searchTerms, startIndex, count);
            }

            var items = result.Value.Select(item => Response.ConvertToSyndicationItem(item.Name, item.Snippet, item.DisplayUrl.ToString(), new Uri(item.Url))).ToList();

            return Response.ConvertToOpenSearchResponse(items, (int)result.TotalEstimatedMatches, searchTerms, startIndex, count);
        }

        private static async Task<Bing.WebWebAnswer> QueryBing(string searchTerms, int startIndex, int count)
        {
            var bingApiKey = ConfigurationManager.AppSettings["BingApiKey"];
            var credentials = new ApiKeyServiceClientCredentials(bingApiKey);
            var client = new WebSearchAPI(credentials);
            var query = string.Format("site:parliament.uk {0}", searchTerms);
            var filter = new List<string> { "Webpages" };
            var response = await client.Web.SearchAsync(
                query,
                responseFilter: filter,
                offset: startIndex - 1,
                count: count,
                market: "en-GB",
                textDecorations: true,
                textFormat: "HTML");

            return response.WebPages;
        }
    }
}
