// MIT License
//
// Copyright (c) 2019 UK Parliament
//
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

namespace OpenSearch
{
    using System.Collections.Generic;
    using System.Linq;
    using System.ServiceModel.Syndication;

    public static class Response
    {
        public static SyndicationItem ConvertToSyndicationItem(string title, string content, string linkTitle, System.Uri linkUri)
        {
            var item = new SyndicationItem
            {
                Title = new TextSyndicationContent(title, TextSyndicationContentKind.Plaintext),
                Content = new TextSyndicationContent(content, TextSyndicationContentKind.Plaintext)
            };

            item.Links.Add(new SyndicationLink
            {
                Title = linkTitle,
                Uri = linkUri
            });

            return item;
        }

        public static Feed ConvertToOpenSearchResponse(IEnumerable<SyndicationItem> items, int totalResults, string searchTerms, int startIndex, int count)
        {
            var result = new Feed
            {
                Title = new TextSyndicationContent(string.Format("parliament.uk search: {0}", searchTerms)),
                TotalResults = totalResults,
                StartIndex = startIndex,
            };

            if (items != null)
            {
                result.Items = items;
            }

            result.ItemsPerPage = result.Items.Count();

            result.Generator = "https://github.com/ukparliament/Search";

            result.Authors.Add(new SyndicationPerson
            {
                Name = "parliament.uk"
            });

            result.Queries.Add(new Query
            {
                Role = Roles.Request,
                SearchTerms = searchTerms,
                StartIndex = startIndex,
                Count = count,
                TotalResults = result.TotalResults
            });

            return result;
        }
    }
}
