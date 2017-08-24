namespace Parliament.Search.Api.Controllers
{
    using OpenSearch;
    using System;
    using System.Configuration;
    using System.Web;
    using System.Web.Http;

    public class DescriptionController : ApiController
    {
        public Description Get()
        {
            var description = new Description
            {
                ShortName = "parliament.uk",
                DescriptionText = "UK Parliament"
            };

            var route = this.Url.Route(
                "NamedController",
                new
                {
                    controller = "search",
                    q = "{searchTerms}",
                    start = "{startIndex?}",
                    count = "{count?}"
                });

            var templateUri = new Uri(new Uri(ConfigurationManager.AppSettings["ApiManagementServiceUrl"]), new Uri(HttpUtility.UrlDecode(route), UriKind.Relative));

            description.Urls.Add(new Url
            {
                TypeName = "application/atom+xml",
                Template = templateUri.ToString()
            });

            description.Urls.Add(new Url
            {
                TypeName = "application/rss+xml",
                Template = templateUri.ToString()
            });

            description.Urls.Add(new Url
            {
                TypeName = "application/json",
                Template = templateUri.ToString()
            });

            description.Urls.Add(new Url
            {
                TypeName = "text/json",
                Template = templateUri.ToString()
            });

            description.Urls.Add(new Url
            {
                TypeName = "text/xml",
                Template = templateUri.ToString()
            });

            description.Urls.Add(new Url
            {
                TypeName = "application/xml",
                Template = templateUri.ToString()
            });

            return description;
        }
    }
}