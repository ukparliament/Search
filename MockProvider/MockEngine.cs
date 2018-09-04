namespace MockProvider
{
    using Library;
    using Parliament.Search.OpenSearch;
    using System;
    using System.Linq;
    using System.ServiceModel.Syndication;
    using System.Threading.Tasks;

    public class MockEngine : IEngine
    {
        public async Task<Feed> Query(string searchTerms, int startIndex, int count)
        {
            var random = new Random();

            var result = new Feed
            {
                TotalResults = random.Next(100, 100000),
                StartIndex = startIndex,
            };

            result.Authors.Add(new SyndicationPerson
            {
                Name = "parliament.uk"
            });

            result.Queries.Add(new Parliament.Search.OpenSearch.Query
            {
                Role = "request",
                SearchTerms = searchTerms,
                StartIndex = startIndex,
                Count = count,
                TotalResults = result.TotalResults
            });

            result.Items = Enumerable.Range(0, random.Next(count)).Select(index =>
            {
                var item = new SyndicationItem
                {
                    Title = new TextSyndicationContent(new string(Enumerable.Repeat('a', random.Next(10, 100)).ToArray()), TextSyndicationContentKind.Html),
                    Summary = new TextSyndicationContent(new string(Enumerable.Repeat('a', random.Next(100, 1000)).ToArray()), TextSyndicationContentKind.Html)
                };

                item.Links.Add(new SyndicationLink
                {
                    Uri = new Uri(new Uri("http://example.com"), new string(Enumerable.Repeat('a', random.Next(5, 15)).ToArray())),
                    MediaType = new string(Enumerable.Repeat('a', random.Next(5, 10)).ToArray()),
                    RelationshipType = "alternate"
                });

                return item;
            });

            return result;
        }
    }
}