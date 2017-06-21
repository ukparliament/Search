namespace GoogleProvider
{
    using Google.Apis.Customsearch.v1;
    using Google.Apis.Customsearch.v1.Data;
    using Google.Apis.Services;
    using Library;
    using Microsoft.ApplicationInsights;
    using Parliament.Search.OpenSearch;
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Linq;
    using System.ServiceModel.Syndication;

    public class GoogleEngine : IEngine
    {
        public Feed Search(string searchTerms, int startIndex, int count)
        {

            if (startIndex <= 0)
            {
                throw new ArgumentOutOfRangeException("startIndex", startIndex, "Allowed values are >=1.");
            }

            if (count > 100 | count < 1)
            {
                throw new ArgumentOutOfRangeException("count", count, "Allowed values are 1-100 inclusive.");
            }

            var maxAllowed = 10;
            var searchList = new List<Search>();
            var firstSearchResult = QueryGoogle(searchTerms, startIndex, Math.Min(maxAllowed, count));
            var totalResults = (int)firstSearchResult.SearchInformation.TotalResults;
            var maxResults = Math.Min(count, totalResults);

            searchList.Add(firstSearchResult);

            if (count > maxAllowed)
            {
                for (int i = maxAllowed + startIndex; i <= maxResults; i += maxAllowed)
                {
                    int remainder = maxResults - i + 1;
                    if (remainder < maxAllowed)
                    {
                        searchList.Add(QueryGoogle(searchTerms, i, remainder));
                    }
                    else
                    {
                        searchList.Add(QueryGoogle(searchTerms, i, maxAllowed));
                    }
                }
            }

            var feed = ConvertToOpenSearchResponse(searchList, searchTerms, startIndex, count);

            var telemetry = new TelemetryClient();

            telemetry.TrackMetric("TotalResults", feed.TotalResults, new Dictionary<string, string> {
                { "searchTerms", searchTerms },
                { "startIndex", startIndex.ToString() },
                { "count", count.ToString() }
            });

            return feed;
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
                Search response = null;
                var telemetry = new TelemetryClient();
                var timer = System.Diagnostics.Stopwatch.StartNew();
                var startTime = DateTime.UtcNow;
                try
                {
                    response = listRequest.Execute();
                }
                finally
                {
                    timer.Stop();
                    telemetry.TrackDependency("Google CSE", searchTerms, startTime, timer.Elapsed, response != null);
                }
                return response;
            }
        }

        private static Feed ConvertToOpenSearchResponse(IEnumerable<Search> searchList, string searchTerms, int startIndex, int count)
        {

            var openSearchResponse = new Response();

            var items = new List<SyndicationItem>();

            var totalResults = 0;

            foreach (Search search in searchList)
            {
                if (totalResults == 0)
                {
                    totalResults = (int)search.SearchInformation.TotalResults;
                }
                if (search.Items != null)
                {
                    items.AddRange(search.Items.Select(item => openSearchResponse.ConvertToSyndicationItem(item.Title, item.Snippet, item.DisplayLink, new Uri(item.Link))));
                }
            }

            return openSearchResponse.ConvertToOpenSearchResponse(items, totalResults, searchTerms, startIndex, count);
        }
    }
}