namespace Parliament.Search.OpenSearch
{
    using System.Collections.Generic;
    using System.Linq;
    using System.ServiceModel.Syndication;

    public class Response
    {
        public SyndicationItem ConvertToSyndicationItem(string title, string content, string linkTitle, System.Uri linkUri)
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

        public Feed ConvertToOpenSearchResponse(IEnumerable<SyndicationItem> items, int totalResults, string searchTerms, int startIndex, int count)
        {
            var result = new Feed()
            {
                Title = new TextSyndicationContent(string.Format("Parliament.uk Search: {0}", searchTerms)),
                TotalResults = totalResults,
                StartIndex = startIndex,
                Items = items
            };

            result.ItemsPerPage = result.Items.Count();

            result.Authors.Add(new SyndicationPerson
            {
                Name = "Parliament.uk"
            });

            result.Queries.Add(new Query
            {
                Role = "request",
                SearchTerms = searchTerms,
                StartIndex = startIndex,
                Count = count,
                TotalResults = result.TotalResults
            });

            return result;
        }
    }
}
