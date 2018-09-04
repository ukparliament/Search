namespace Parliament.Search.Api.Controllers
{
    using System.Web.Http;
    using OpenSearch;

    [DescriptionControllerConfiguration]
    public class DescriptionController : ApiController
    {
        [Route("description")]
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

            description.Urls.Add(new Url
            {
                TypeName = "application/atom+xml",
                Template = this.Url.Link("QueryWithExtension", new { ext = "atom", q = "{searchTerms}", start = "{startIndex?}", count = "{count?}" }).Replace("%7B", "{").Replace("%7D", "}").Replace("%3F", "?")
            });

            description.Urls.Add(new Url
            {
                TypeName = "application/rss+xml",
                Template = this.Url.Link("QueryWithExtension", new { ext = "rss", q = "{searchTerms}", start = "{startIndex?}", count = "{count?}" }).Replace("%7B", "{").Replace("%7D", "}").Replace("%3F", "?")
            });

            description.Urls.Add(new Url
            {
                TypeName = "application/json",
                Template = this.Url.Link("QueryWithExtension", new { ext = "json", q = "{searchTerms}", start = "{startIndex?}", count = "{count?}" }).Replace("%7B", "{").Replace("%7D", "}").Replace("%3F", "?")
            });

            description.Urls.Add(new Url
            {
                TypeName = "text/html",
                Template = this.Url.Link("QueryWithExtension", new { ext = "html", q = "{searchTerms}" }).Replace("%7B", "{").Replace("%7D", "}").Replace("%3F", "?")
            });

            return description;
        }
    }
}