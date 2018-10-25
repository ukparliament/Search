namespace OpenSearch
{
    using System.Xml;
    using System.Xml.Serialization;

    public static class Constants
    {
        /// <summary>
        /// The XML Namespaces URI for the XML data formats described in the OpenSearch specification.
        /// </summary>
        public const string NamespaceUri = "http://a9.com/-/spec/opensearch/1.1/";
        public const string DescriptionMimeType = "application/opensearchdescription+xml";
        public const string Prefix = "os";

        public static XmlSerializerNamespaces Namespaces
        {
            get
            {
                return new XmlSerializerNamespaces(new XmlQualifiedName[] {
                    new XmlQualifiedName(
                        string.Empty,
                        Constants.NamespaceUri)
                });
            }
        }
    }

    public class Relation
    {
        public const string Results = "results";
        public const string Suggestions = "suggestions";
        public const string Self = "self";
        public const string Collection = "collection";
    }

    public class Roles
    {
        public const string Example = "example";
        public const string Request = "request";
    }
}
