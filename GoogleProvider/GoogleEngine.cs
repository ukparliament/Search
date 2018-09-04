namespace GoogleProvider
{
    using Google.Apis.Customsearch.v1;
    using Google.Apis.Customsearch.v1.Data;
    using Google.Apis.Services;
    using Library;
    using Parliament.Search.OpenSearch;
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Linq;
    using System.ServiceModel.Syndication;
    using System.Threading.Tasks;

    public class GoogleEngine : IEngine
    {
        public async Task<Feed> Query(string searchTerms, int startIndex, int count)
        {
            var items = new List<SyndicationItem>();

            var GoogleMaxResultsPerQuery = 10;

            var result = QueryGoogle(searchTerms, startIndex, Math.Min(GoogleMaxResultsPerQuery, count));

            var totalResults = (int)result.SearchInformation.TotalResults;

            items.AddRange(result.Items.Select(item => Response.ConvertToSyndicationItem(item.Title, item.Snippet, item.DisplayLink, new Uri(item.Link))));

            for (int nextIndex = startIndex + items.Count(); nextIndex <= totalResults && items.Count() < count; nextIndex += GoogleMaxResultsPerQuery)
            {
                var nextCount = Math.Min(GoogleMaxResultsPerQuery, count - items.Count());

                var nextResult = QueryGoogle(searchTerms, nextIndex, nextCount);

                if (nextResult.Items.Count() == 0)
                {
                    break;
                }
                items.AddRange(nextResult.Items.Select(item => Response.ConvertToSyndicationItem(item.Title, item.Snippet, item.DisplayLink, new Uri(item.Link))));

            }

            return Response.ConvertToOpenSearchResponse(items, totalResults, searchTerms, startIndex, count);
        }

        private static Search QueryGoogle(string searchTerms, int startIndex, int count)
        {

            var initializer = new BaseClientService.Initializer()
            {
                ApiKey = ConfigurationManager.AppSettings["GoogleApiKey"]
            };

            using (var service = new CustomsearchService(initializer))
            {
                var listRequest = service.Cse.List(searchTerms);
                listRequest.Start = startIndex;
                listRequest.Num = count;
                listRequest.Cx = ConfigurationManager.AppSettings["GoogleEngineId"];

                return listRequest.Execute();
            }
        }
    }
}