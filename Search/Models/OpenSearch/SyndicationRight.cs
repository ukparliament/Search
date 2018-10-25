namespace OpenSearch
{
    using System.Xml.Serialization;

    public enum SyndicationRight
    {
        /// <summary>
        /// <list>The search client may request search results.</list>
        /// <list>The search client may display the search results to end users.</list>
        /// <list>The search client may send the search results to other search clients.</list>
        /// </summary>
        [XmlEnum("open")]
        Open,
        /// <summary>
        /// <list>The search client may request search results.</list>
        /// <list>The search client may display the search results to end users.</list>
        /// <list>The search client may not send the search results to other search clients.</list>
        /// </summary>
        [XmlEnum("limited")]
        Limited,
        /// <summary>
        /// <list>The search client may request search results.</list>
        /// <list>The search client may not display the search results to end users.</list>
        /// <list>The search client may not send the search results to other search clients.</list>
        /// </summary>
        [XmlEnum("private")]
        Private,
        /// <summary>
        /// <list>The search client may not request search results.</list>
        /// </summary>
        [XmlEnum("closed")]
        Closed
    }
}
