namespace Parliament.Search.OpenSearch
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
