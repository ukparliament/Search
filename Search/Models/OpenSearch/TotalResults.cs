namespace OpenSearch
{
    using System.Xml.Serialization;

    [XmlRoot("totalResults", Namespace = Constants.NamespaceUri)]
    public class TotalResults
    {
        [XmlText]
        public int Value { get; set; }
    }
}
