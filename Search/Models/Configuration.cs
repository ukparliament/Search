namespace Search
{
    using System;
    using System.IO;
    using System.ServiceModel.Syndication;

    public class Configuration
    {
        public static readonly (string MediaType, string Extension, Action<SyndicationFeed, TextWriter> writeMethod)[] Mappings = new (string, string, Action<SyndicationFeed, TextWriter>)[] {
            ("application/atom+xml", "atom", FeedFormatter.WriteAtom),
            ("application/rss+xml", "rss", FeedFormatter.WriteRss),
            ("text/html", "html", null),
            ("application/json", "json", FeedFormatter.WriteJson)
        };

        public string Engine { get; set; }
    }
}
