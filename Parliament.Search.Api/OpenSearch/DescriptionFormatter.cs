// TODO: Move to library?

// Based on http://www.strathweb.com/2012/04/rss-atom-mediatypeformatter-for-asp-net-webapi/

namespace Parliament.OpenSearch
{
    using System;
    using System.IO;
    using System.Net.Http;
    using System.Net.Http.Formatting;
    using System.Xml.Serialization;
    using Net.Http.Formatting;
    using Search.OpenSearch;

    public class DescriptionFormatter : BufferedMediaTypeFormatter
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

        public override void WriteToStream(Type type, object value, Stream writeStream, HttpContent content)
        {
            new XmlSerializer(type).Serialize(writeStream, value, Constants.Namespaces);
        }
    }
}