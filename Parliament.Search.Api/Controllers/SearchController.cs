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
            
            List<Search> searchList = new List<Search>();

            while (count >= 10)
            {
                searchList.Add(SearchController.Query(
                    searchTerms,
                    startIndex,
                    10));
                startIndex += 10;
                count -= 10;
            }

            if (count > 0)
            {
                searchList.Add(SearchController.Query(
                    searchTerms,
                    startIndex,
                    count));
            }

            var feed = SearchController.ConvertResults(searchList);

            var telemetry = new TelemetryClient();

            telemetry.TrackMetric("TotalResults", feed.TotalResults, new Dictionary<string, string> { { "searchTerms", searchTerms }, { "startIndex", startIndex.ToString() }, { "count", count.ToString() } });

            return feed;
        }

        private static Feed ConvertResults(List<Search> searchList)
        {

            var items = new List<SyndicationItem>();

            var result = new Feed();

            foreach (Search search in searchList)
            {
                if (result.TotalResults == 0)
                {
                    result.TotalResults = (int)search.SearchInformation.TotalResults;
                    result.StartIndex = search.Queries["request"].SingleOrDefault().StartIndex??0;
                    result.Queries.Add(new OpenSearch.Query
                    {
                        Role = "request",
                        SearchTerms = search.Queries["request"].SingleOrDefault().SearchTerms

                    });

                }
                if (search.Items != null)
                {
                    items.AddRange(search.Items.Select(ConvertItem));
                }
            }


            result.Items = items;


            result.Authors.Add(new SyndicationPerson
            {
                Name = "parliament.uk"
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
            if (startIndex <= 0)
            {
                throw new ArgumentOutOfRangeException("startIndex", startIndex, "Allowed values are >=1.");
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