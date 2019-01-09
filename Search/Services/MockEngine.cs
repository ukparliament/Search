// MIT License
//
// Copyright (c) 2019 UK Parliament
//
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

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