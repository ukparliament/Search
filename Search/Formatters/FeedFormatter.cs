namespace Search
{
    using System;
    using System.IO;
    using System.ServiceModel.Syndication;
    using System.Text;
    using System.Threading.Tasks;
    using System.Xml;
    using System.Xml.Xsl;
    using Microsoft.AspNetCore.Mvc.Formatters;
    using Newtonsoft.Json;

    internal class FeedFormatter : TextOutputFormatter
    {
        private readonly Action<SyndicationFeed, TextWriter> writeMethod;

        public FeedFormatter(string mediaType, Action<SyndicationFeed, TextWriter> writeMethod)
        {
            this.SupportedMediaTypes.Add(mediaType);
            this.SupportedEncodings.Add(Encoding.UTF8);
            this.writeMethod = writeMethod;
        }

        protected override bool CanWriteType(Type type)
        {
            return typeof(SyndicationFeed).IsAssignableFrom(type);
        }

        public override Task WriteResponseBodyAsync(OutputFormatterWriteContext context, Encoding selectedEncoding)
        {
            return new TaskFactory().StartNew(() =>
            {
                using (var writer = context.WriterFactory(context.HttpContext.Response.Body, selectedEncoding))
                {
                    this.writeMethod.Invoke(context.Object as SyndicationFeed, writer);
                }
            });
        }

        internal static void WriteAtom(SyndicationFeed feed, TextWriter writer)
        {
            FeedFormatter.Write(feed.SaveAsAtom10, writer);
        }

        internal static void WriteRss(SyndicationFeed feed, TextWriter writer)
        {
            FeedFormatter.Write(feed.SaveAsRss20, writer);
        }

        internal static void WriteJson(SyndicationFeed feed, TextWriter writer)
        {
            new JsonSerializer { NullValueHandling = NullValueHandling.Ignore }.Serialize(writer, feed);
        }

        private static void Write(Action<XmlWriter> write, TextWriter writer)
        {
            var settings = new XmlWriterSettings { Encoding = new UTF8Encoding(false) };
            using (var xmlWriter = XmlWriter.Create(writer, settings))
            {
                write(xmlWriter);
            }
        }
    }
}
