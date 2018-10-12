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

            foreach (var mapping in Configuration.Mappings)
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