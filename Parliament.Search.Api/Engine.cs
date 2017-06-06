namespace Parliament.Search.Api
{
    using System;
    using Parliament.Search.OpenSearch;
    using System.Collections.Generic;
    using Google.Apis.Services;
    using System.Configuration;
    using Google.Apis.Customsearch.v1;
    using Microsoft.ApplicationInsights;
    using Google.Apis.Customsearch.v1.Data;
    using System.ServiceModel.Syndication;
    using System.Linq;

    internal class Engine : IEngine
    {
        public Feed Search(string searchTerms, int startIndex, int pageSize)
        {

            if (startIndex <= 0)
            {
                throw new ArgumentOutOfRangeException("startIndex", startIndex, "Allowed values are >=1.");
            }

            if (pageSize > 100 | pageSize < 1)
            {
                throw new ArgumentOutOfRangeException("pageSize", pageSize, "Allowed values are 1-100 inclusive.");
            }

            var maxAllowed = 10;
            var searchList = new List<Search>();
            var firstSearchResult = Query(searchTerms, startIndex, Math.Min(maxAllowed, pageSize));
            var totalResults = (int)firstSearchResult.SearchInformation.TotalResults;
            var maxResults = Math.Min(pageSize, totalResults);

            searchList.Add(firstSearchResult);

            if (pageSize > maxAllowed)
            {
                for (int i = maxAllowed + startIndex; i <= maxResults; i += maxAllowed)
                {
                    int remainder = maxResults - i + 1;
                    if (remainder < maxAllowed)
                    {
                        searchList.Add(Engine.Query(searchTerms, i, remainder));
                    }
                    else
                    {
                        searchList.Add(Engine.Query(searchTerms, i, maxAllowed));
                    }
                }
            }

            var feed = Engine.ConvertResults(searchList, searchTerms, startIndex, pageSize);

            var telemetry = new TelemetryClient();

            telemetry.TrackMetric("TotalResults", feed.TotalResults, new Dictionary<string, string> {
                { "searchTerms", searchTerms },
                { "startIndex", startIndex.ToString() },
                { "pageSize", pageSize.ToString() }
            });

            return feed;
        }

        private static Search Query(string searchTerms, int startIndex, int pageSize)
        {

            var initializer = new BaseClientService.Initializer();
            initializer.ApiKey = ConfigurationManager.AppSettings["GoogleApiKey"];

            using (var service = new CustomsearchService(initializer))
            {
                var listRequest = service.Cse.List(searchTerms);
                listRequest.Start = startIndex;
                listRequest.Num = pageSize;
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

        private static Feed ConvertResults(IEnumerable<Search> searchList, string searchTerms, int startIndex, int pageSize)
        {
            var result = new Feed();

            var items = new List<SyndicationItem>();

            foreach (Search search in searchList)
            {
                if (result.TotalResults == 0)
                {
                    result.TotalResults = (int)search.SearchInformation.TotalResults;
                }
                if (search.Items != null)
                {
                    items.AddRange(search.Items.Select(ConvertItem));
                }
            }

            result.Items = items;
            result.StartIndex = startIndex;

            result.Authors.Add(new SyndicationPerson
            {
                Name = "parliament.uk"
            });

            result.Queries.Add(new OpenSearch.Query
            {
                Role = "request",
                SearchTerms = searchTerms,
                StartIndex = startIndex,
                Count = pageSize,
                TotalResults = result.TotalResults
            });

            return result;
        }

        private static SyndicationItem ConvertItem(Result item)
        {
            var newItem = new SyndicationItem
            {
                Title = new TextSyndicationContent(item.HtmlTitle, TextSyndicationContentKind.Html),
                Summary = new TextSyndicationContent(item.HtmlSnippet, TextSyndicationContentKind.Html)
            };

            newItem.Links.Add(new SyndicationLink
            {
                Uri = new Uri(item.Link),
                MediaType = item.Mime,
                RelationshipType = "alternate"
            });

            return newItem;
        }
    }
}