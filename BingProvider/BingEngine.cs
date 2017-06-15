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
            var feed = BingEngine.ConvertToOpenSearch(bingResponse, searchTerms, startIndex, pageSize);

            return feed;
        }

        private static Feed ConvertToOpenSearch(BingResponse bingResponse, string searchTerms, int startIndex, int pageSize)
        {
            var result = new Feed()
            {
                Title = new TextSyndicationContent(string.Format("Parliament.uk: {0}", searchTerms)),
                TotalResults = bingResponse.WebPages.TotalEstimatedMatches,
                StartIndex = startIndex,
                Items = bingResponse.WebPages.Values.Select(value =>
                {
                    var item = new SyndicationItem
                    {
                        Title = new TextSyndicationContent(value.Name),
                        Content = new TextSyndicationContent(value.Snippet, TextSyndicationContentKind.Html),
                        LastUpdatedTime = new System.DateTimeOffset(value.DateLastCrawled)
                    };

                    item.Links.Add(new SyndicationLink
                    {
                        Title = value.DisplayUrl.ToString(),
                        Uri = value.Uri
                    });

                    return item;
                })
            };

            result.Authors.Add(new SyndicationPerson
            {
                Name = "Parliament.uk"
            });

            result.Queries.Add(new Parliament.Search.OpenSearch.Query
            {
                Role = "request",
                SearchTerms = searchTerms,
                StartIndex = startIndex,
                Count = pageSize,
                TotalResults = result.TotalResults
            });

            return result;
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
