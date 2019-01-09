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
    using System.Net;
    using Microsoft.AspNetCore.Mvc;
    using OpenSearch;

    public class DescriptionController : ControllerBase
    {
        [HttpGet("description", Name = "Description")]
        [Produces(Constants.DescriptionMimeType)]
        public Description Get()
        {
            var description = new Description
            {
                ShortName = "parliament.uk",
                DescriptionText = "UK Parliament",
                Contact = "data@parliament.uk",
                Developer = "https://twitter.com/UKParliData"
            };

            description.Queries.Add(new Query
            {
                Role = Roles.Example,
                SearchTerms = "historic hansard"
            });

            description.Urls.Add(new Url
            {
                TypeName = Constants.DescriptionMimeType,
                Relation = Relation.Self,
                Template = this.Url.Link("Description", null)
            });

            foreach (var mapping in Configuration.QueryMappings)
            {
                description.Urls.Add(new Url
                {
                    TypeName = mapping.MediaType,
                    Template = WebUtility.UrlDecode(this.Url.Link("QueryWithExtension", new { format = mapping.Extension, q = "{searchTerms}", start = "{startIndex?}", count = "{count?}" }))
                });
            }

            return description;
        }
    }
}