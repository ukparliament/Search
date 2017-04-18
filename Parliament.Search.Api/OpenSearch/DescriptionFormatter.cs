// TODO: Move to library

// Based on http://www.strathweb.com/2012/04/rss-atom-mediatypeformatter-for-asp-net-webapi/

namespace Parliament.OpenSearch
{
    using Net.Http.Formatting;
    using Search.OpenSearch;
    using System;
    using System.IO;
    using System.Net;
    using System.Net.Http;
    using System.Net.Http.Formatting;
    using System.Threading.Tasks;
    using System.Xml.Serialization;
    using System.Xml.Serialization;

    public class DescriptionFormatter : MediaTypeFormatter
    {
        public DescriptionFormatter() : base()
        {
            this.MediaTypeMappings.Add(new StaticMediaTypeMapping(Constants.DescriptionMimeType));
        }

        public override bool CanReadType(Type type)
        {
            return false;
        }

        public override bool CanWriteType(Type type)
        {
            return typeof(Description).IsAssignableFrom(type);
        }

        public override Task WriteToStreamAsync(Type type, object value, Stream writeStream, HttpContent content, TransportContext transportContext)
        {
            return Task.Factory.StartNew(() =>
            {
                var serializer = new XmlSerializer(type);
                serializer.Serialize(writeStream, value, Constants.Namespaces);
            });
        }
    }
}