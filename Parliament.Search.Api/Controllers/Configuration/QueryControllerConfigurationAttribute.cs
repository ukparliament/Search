namespace Parliament.Search.Api.Controllers
{
    using System;
    using System.Net.Http;
    using System.Net.Http.Formatting;
    using System.Web.Http.Controllers;
    using Parliament.ServiceModel.Syndication;

    [AttributeUsage(AttributeTargets.Class)]
    public sealed class QueryControllerConfigurationAttribute : Attribute, IControllerConfiguration
    {
        public void Initialize(HttpControllerSettings controllerSettings, HttpControllerDescriptor controllerDescriptor)
        {
            controllerSettings.Formatters.Add(new FeedFormatter(new UriPathExtensionMapping("atom", "application/atom+xml"), FeedFormatter.WriteAtom));
            controllerSettings.Formatters.Add(new FeedFormatter(new UriPathExtensionMapping("rss", "application/rss+xml"), FeedFormatter.WriteRss));
            controllerSettings.Formatters.Add(new FeedFormatter(new UriPathExtensionMapping("xml", "application/atom+xml"), FeedFormatter.WriteAtom));
            controllerSettings.Formatters.Add(new FeedFormatter(new UriPathExtensionMapping("html", "text/html"), FeedFormatter.WriteHtml));
            controllerSettings.Formatters.Add(new FeedFormatter(new UriPathExtensionMapping("json", "application/json"), FeedFormatter.WriteJson));

            controllerSettings.Formatters.Add(new FeedFormatter(new RequestHeaderMapping("Accept", "application/atom+xml", StringComparison.OrdinalIgnoreCase, false, "application/atom+xml"), FeedFormatter.WriteAtom));
            controllerSettings.Formatters.Add(new FeedFormatter(new RequestHeaderMapping("Accept", "application/rss+xml", StringComparison.OrdinalIgnoreCase, false, "application/rss+xml"), FeedFormatter.WriteRss));
            controllerSettings.Formatters.Add(new FeedFormatter(new RequestHeaderMapping("Accept", "text/xml", StringComparison.OrdinalIgnoreCase, false, "application/atom+xml"), FeedFormatter.WriteAtom));
            controllerSettings.Formatters.Add(new FeedFormatter(new RequestHeaderMapping("Accept", "application/xml", StringComparison.OrdinalIgnoreCase, false, "application/atom+xml"), FeedFormatter.WriteAtom));
            controllerSettings.Formatters.Add(new FeedFormatter(new RequestHeaderMapping("Accept", "text/html", StringComparison.OrdinalIgnoreCase, false, "text/html"), FeedFormatter.WriteHtml));
            controllerSettings.Formatters.Add(new FeedFormatter(new RequestHeaderMapping("Accept", "application/json", StringComparison.OrdinalIgnoreCase, false, "application/json"), FeedFormatter.WriteJson));
        }
    }
}
