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
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.Azure.CognitiveServices.Search.WebSearch;
    using Microsoft.Extensions.Configuration;
    using OpenSearch;
    using Bing = Microsoft.Azure.CognitiveServices.Search.WebSearch.Models;

    public class BingEngine : IEngine
    {
        private readonly BingConfiguration configuration;

        public BingEngine(IConfiguration configuration)
        {
            this.configuration = configuration.GetSection("BingEngine").Get<BingConfiguration>();
        }

        public async Task<Feed> Query(string searchTerms, int startIndex, int count, string subdomains, string inUrlPrefixes)
        {
            if (count > 50)
            {
                throw new ArgumentOutOfRangeException(nameof(count), count, "max 50");
            }

            var result = await this.QueryBing(searchTerms, startIndex, count, subdomains, inUrlPrefixes);

            if (result == null)
            {
                return Response.ConvertToOpenSearchResponse(null, 0, searchTerms, startIndex, count);
            }

            var items = result.Value.Select(item => Response.ConvertToSyndicationItem(item.Name, item.Snippet, item.DisplayUrl.ToString(), new Uri(item.Url))).ToList();

            return Response.ConvertToOpenSearchResponse(items, (int)result.TotalEstimatedMatches, searchTerms, startIndex, count);
        }

        private async Task<Bing.WebWebAnswer> QueryBing(string searchTerms, int startIndex, int count, string subdomains, string inUrlPrefixes)
        {
            using (var client = new WebSearchClient(new ApiKeyServiceClientCredentials(this.configuration.SubscriptionKey)))
            {
                string query = null;
                if (subdomains == null)
                {
                    query = string.Format("site:parliament.uk {0}", searchTerms);
                }
                else
                {
                    var sites = subdomains.Trim().Split(',').Select(site => site.Trim());
                    query = searchTerms;
                    int index = 0;
                    foreach (var site in sites)
                    {
                        if (!site.ToLower().Contains("parliament.uk"))
                        {
                            if (index == (sites.Count() - 1))
                            {
                                query += string.Format(" site:{0}.parliament.uk", site);
                            }
                            else
                            {
                                query += string.Format(" site:{0}.parliament.uk OR ", site);
                            }
                        }
                        else
                        {
                            if (index == (sites.Count() - 1))
                            {
                                query += string.Format(" site:{0}", site);
                            }
                            else
                            {
                                query += string.Format(" site:{0} OR ", site);
                            }
                        }

                        index++;
                    }
                }

                if (inUrlPrefixes != null)
                {
                    var prefixes = inUrlPrefixes.Trim().Split(",").Select(prefix => prefix.Trim());
                    if (prefixes.Count() == 1)
                    {
                        query += string.Format(" instreamset:url:{0})", prefixes.First());
                    }
                    else
                    {
                        int index = 0;
                        string subQuery = " (";
                        foreach (var prefix in prefixes)
                        {
                            if (index == (prefixes.Count() - 1))
                            {
                                subQuery += string.Format(" instreamset:url:{0})", prefix);
                            }
                            else
                            {
                                subQuery += string.Format(" instreamset:url:{0} OR ", prefix);
                            }

                            index++;
                        }

                        query += subQuery;
                    }
                }

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
}
