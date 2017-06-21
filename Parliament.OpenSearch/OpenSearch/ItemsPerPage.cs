namespace Parliament.Search.OpenSearch
{
    using System.Xml.Serialization;

    [XmlRoot("itemsPerPage", Namespace = Constants.NamespaceUri)]
    public class ItemsPerPage
    {
        [XmlText()]
        public int Value { get; set; }
    }
}
