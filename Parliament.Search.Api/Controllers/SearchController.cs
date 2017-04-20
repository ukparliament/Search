namespace Parliament.Search.Api.Controllers
{
    using Google.Apis.Customsearch.v1;
    using Google.Apis.Customsearch.v1.Data;
    using Google.Apis.Services;
    using Microsoft.ApplicationInsights;
    using OpenSearch;
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Linq;
    using System.ServiceModel.Syndication;
    using System.Web.Http;

    public class SearchController : ApiController
    {
        public Feed Get([FromUri(Name = "q")]string searchTerms)
        {
            return Get(searchTerms, 1);
        }

        public Feed Get([FromUri(Name = "q")]string searchTerms, [FromUri(Name = "start")]int startIndex)
        {
            return Get(searchTerms, startIndex, 10);
        }

        public Feed Get([FromUri(Name = "q")]string searchTerms, [FromUri(Name = "start")]int startIndex, [FromUri(Name = "pagesize")]int count)
        {
 
            var feed = SearchController.ConvertResults(
                SearchController.Query(
                    searchTerms,
                    startIndex,
                    count));

            var telemetry = new TelemetryClient();

            telemetry.TrackMetric("TotalResults", feed.TotalResults, new Dictionary<string, string> { { "searchTerms", searchTerms }, { "startIndex", startIndex.ToString() }, { "count", count.ToString() } });

            return feed;
        }

        private static Feed ConvertResults(Search search)
        {
            var request = search.Queries["request"].Single();

            var result = new Feed
            {
                Items = search.Items.Select(ConvertItem),
                TotalResults = (int)search.SearchInformation.TotalResults,
                StartIndex = (int)request.StartIndex
            };

            result.Authors.Add(new SyndicationPerson
            {
                Name = "parliament.uk"
            });

            result.Queries.Add(new OpenSearch.Query
            {
                Role = "request",
                SearchTerms = request.SearchTerms
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

        private static Search Query(string searchTerms, int startIndex, int count)
        {
            if (count > 10)
            {
                throw new ArgumentOutOfRangeException("count", count, "Allowed values are 1 to 10 inclusive.");
            }

            var initializer = new BaseClientService.Initializer();
            initializer.ApiKey = ConfigurationManager.AppSettings["GoogleApiKey"];

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