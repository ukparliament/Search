// TODO: Move to library

// Based on http://www.strathweb.com/2012/04/rss-atom-mediatypeformatter-for-asp-net-webapi/

namespace Parliament.ServiceModel.Syndication
{
    using Microsoft.ApplicationInsights;
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Net;
    using System.Net.Http;
    using System.Net.Http.Formatting;
    using System.Net.Http.Headers;
    using System.ServiceModel.Syndication;
    using System.Text;
    using System.Threading.Tasks;
    using System.Xml;

    public class FeedFormatter : MediaTypeFormatter
    {
        private const string atom = "application/atom+xml";
        private const string rss = "application/rss+xml";
        private const string textXml = "text/xml";
        private const string applicationXml = "application/xml";

        public FeedFormatter() : base()
        {
            this.MediaTypeMappings.Add(new RequestHeaderMapping("Accept", atom, StringComparison.Ordinal, false, new MediaTypeHeaderValue(atom)));
            this.MediaTypeMappings.Add(new RequestHeaderMapping("Accept", rss, StringComparison.Ordinal, false, new MediaTypeHeaderValue(rss)));
            this.MediaTypeMappings.Add(new RequestHeaderMapping("Accept", textXml, StringComparison.Ordinal, false, new MediaTypeHeaderValue(textXml)));
            this.MediaTypeMappings.Add(new RequestHeaderMapping("Accept", applicationXml, StringComparison.Ordinal, false, new MediaTypeHeaderValue(applicationXml)));
        }

        public override bool CanReadType(Type type)
        {
            return this.CanWriteType(type);
        }

        public override bool CanWriteType(Type type)
        {
            return typeof(SyndicationFeed).IsAssignableFrom(type);
        }

        public override Task WriteToStreamAsync(Type type, object value, Stream writeStream, HttpContent content, TransportContext transportContext)
        {
            return Task.Factory.StartNew(() =>
            {
                FeedFormatter.Write(value as SyndicationFeed, writeStream, content.Headers.ContentType.MediaType);
            });
        }

        private static void Write(SyndicationFeed feed, Stream writeStream, string mediaType)
        {
            var writeMethods = new Dictionary<string, Action<XmlWriter>>() {
                { FeedFormatter.atom, feed.SaveAsAtom10 },
                { FeedFormatter.rss, feed.SaveAsRss20 },
                { FeedFormatter.textXml, feed.SaveAsAtom10 },
                { FeedFormatter.applicationXml, feed.SaveAsAtom10 }
            };

            var writeMethod = writeMethods[mediaType];

            var timer = Stopwatch.StartNew();
            var settings = new XmlWriterSettings() { Encoding = new UTF8Encoding(false) };
            using (var writer = XmlWriter.Create(writeStream, settings))
            {
                writeMethod(writer);
            }
            timer.Stop();

            var telemetry = new TelemetryClient();
            telemetry.TrackEvent("FeedFormatter.Write", new Dictionary<string, string> { { "content-type", mediaType } }, new Dictionary<string, double> { { "ElapsedMilliseconds", timer.ElapsedMilliseconds } });
        }
    }
}