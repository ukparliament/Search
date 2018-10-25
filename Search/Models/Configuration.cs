namespace Search
{
    using System;
    using System.IO;
    using System.ServiceModel.Syndication;
    using Microsoft.OpenApi.Writers;

    public class Configuration
    {
        public static readonly (string MediaType, string Extension, Action<SyndicationFeed, TextWriter> writeMethod)[] QueryMappings = new (string, string, Action<SyndicationFeed, TextWriter>)[] {
            ("application/atom+xml", "atom", FeedFormatter.WriteAtom),
            ("application/rss+xml", "rss", FeedFormatter.WriteRss),
            ("text/html", "html", null),
            ("application/json", "json", FeedFormatter.WriteJson)
        };

        internal static readonly (string MediaType, string Extension, Type WriterType)[] OpenApiMappings = new[] {
            ("application/json", "json", typeof(OpenApiJsonWriter)),
            ("text/vnd.yaml", "yaml", typeof(OpenApiYamlWriter))
        };

        public string Engine { get; set; }
    }
}
