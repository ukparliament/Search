namespace Parliament.Search.OpenSearch
{
    using System.Xml.Serialization;

    [XmlRoot("Url", Namespace = Constants.NamespaceUri)]
    public class Url
    {
        [XmlAttribute("template")]
        public string Template { get; set; }

        [XmlAttribute("type")]
        public string TypeName { get; set; }
    }
}
