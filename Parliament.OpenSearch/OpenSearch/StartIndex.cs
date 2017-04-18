namespace Parliament.Search.OpenSearch
{
    using System.Xml.Serialization;

    [XmlRoot("startIndex", Namespace = Constants.NamespaceUri)]
    public class StartIndex
    {
        [XmlText]
        public int Value { get; set; }
    }
}
