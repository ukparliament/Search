namespace Search
{
    using System;
    using System.Linq;
    using System.ServiceModel.Syndication;
    using System.Threading.Tasks;
    using OpenSearch;

    public class MockEngine : IEngine
    {
        public async Task<Feed> Query(string searchTerms, int startIndex, int count)
        {
            var random = new Random();

            var result = new Feed
            {
                Title = new TextSyndicationContent("mock search results"),
                TotalResults = random.Next(100, 100000),
                StartIndex = startIndex,
            };

            result.Authors.Add(new SyndicationPerson
            {
                Name = "parliament.uk"
            });

            result.Queries.Add(new Query
            {
                Role = "request",
                SearchTerms = searchTerms,
                StartIndex = startIndex,
                Count = count,
                TotalResults = result.TotalResults
            });

            result.Items = Enumerable.Range(0, random.Next(count)).Select(index =>
                 Response.ConvertToSyndicationItem(
                    new string(Enumerable.Repeat('a', random.Next(10, 100)).ToArray()),
                    new string(Enumerable.Repeat('a', random.Next(100, 1000)).ToArray()),
                    new string(Enumerable.Repeat('a', random.Next(5, 10)).ToArray()),
                    new Uri(new Uri("http://example.com"), new string(Enumerable.Repeat('a', random.Next(5, 15)).ToArray())))
            );

            return result;
        }
    }
}