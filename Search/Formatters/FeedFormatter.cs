// MIT License
//
// Copyright (c) 2019 UK Parliament
//
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

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
