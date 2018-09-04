// TODO: Move to library

// Based on http://www.strathweb.com/2012/04/rss-atom-mediatypeformatter-for-asp-net-webapi/

namespace Parliament.ServiceModel.Syndication
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Net.Http;
    using System.Net.Http.Formatting;
    using System.ServiceModel.Syndication;
    using System.Text;
    using System.Xml;
    using System.Xml.Xsl;
    using Microsoft.ApplicationInsights;
    using Newtonsoft.Json;
    using Parliament.Search.Api;

    public class FeedFormatter : BufferedMediaTypeFormatter
    {
        private readonly Action<SyndicationFeed, Stream> writeMethod;

        public FeedFormatter(MediaTypeMapping mapping, Action<SyndicationFeed, Stream> writeMethod) : base()
        {
            this.SupportedMediaTypes.Add(mapping.MediaType);
            this.MediaTypeMappings.Add(mapping);
            this.writeMethod = writeMethod;
        }

        public override bool CanReadType(Type type)
        {
            return false;
        }

        public override bool CanWriteType(Type type)
        {
            return typeof(SyndicationFeed).IsAssignableFrom(type);
        }

        public override void WriteToStream(Type type, object value, Stream writeStream, HttpContent content)
        {
            this.writeMethod.Invoke(value as SyndicationFeed, writeStream);

            new TelemetryClient().TrackEvent("FeedFormatter.Write", new Dictionary<string, string> { { "content-type", content.Headers.ContentType.MediaType } });
        }

        internal static void WriteAtom(SyndicationFeed feed, Stream stream)
        {
            FeedFormatter.Write(feed.SaveAsAtom10, stream);
        }

        internal static void WriteRss(SyndicationFeed feed, Stream stream)
        {
            FeedFormatter.Write(feed.SaveAsRss20, stream);
        }

        internal static void WriteHtml(SyndicationFeed feed, Stream stream)
        {
            using (var atomStream = new MemoryStream())
            {
                WriteAtom(feed, atomStream);
                atomStream.Seek(0, SeekOrigin.Begin);
                using (var atomReader = XmlReader.Create(atomStream))
                {
                    var xslt = new XslCompiledTransform();
                    using (var rendererStream = Resources.GetStream("Parliament.Search.Api.AtomHtmlRenderer.xslt"))
                    {
                        using (var stylesheet = XmlReader.Create(rendererStream))
                        {
                            xslt.Load(stylesheet);
                            xslt.Transform(atomReader, null, stream);
                        }
                    }
                }
            }
        }

        internal static void WriteJson(SyndicationFeed feed, Stream stream)
        {
            using (var writer = new StreamWriter(stream))
            {
                new JsonSerializer().Serialize(writer, feed);
            }
        }

        private static void Write(Action<XmlWriter> write, Stream stream)
        {
            var settings = new XmlWriterSettings { Encoding = new UTF8Encoding(false) };
            using (var writer = XmlWriter.Create(stream, settings))
            {
                write(writer);
            }
        }
    }
}