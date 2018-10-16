namespace OpenSearch
{
    using System.Xml.Serialization;

    /// <summary>
    /// The following role values are identified with the OpenSearch 1.1 namespace.
    /// </summary>
    /// <remarks>The list is exhaustive; only the role values listed below may appear in the OpenSearch 1.1 namespace.</remarks>
    /// <seealso cref="http://www.opensearch.org/Specifications/OpenSearch/1.1#Role_values"/> 
    public enum Role
    {
        /// <summary>
        /// Represents the search query that can be performed to retrieve the same set of search results.
        /// </summary>
        [XmlEnum("request")]
        Request,

        /// <summary>
        /// Represents a search query that can be performed to demonstrate the search engine.
        /// </summary>
        [XmlEnum("example")]
        Example,

        /// <summary>
        /// Represents a search query that can be performed to retrieve similar but different search results.
        /// </summary>
        [XmlEnum("related")]
        Related,

        /// <summary>
        /// Represents a search query that can be performed to improve the result set, such as with a spelling correction.
        /// </summary>
        [XmlEnum("correction")]
        Correction,

        /// <summary>
        /// Represents a search query that will narrow the current set of search results.
        /// </summary>
        [XmlEnum("subset")]
        Subset,

        /// <summary>
        /// Represents a search query that will broaden the current set of search results.
        /// </summary>
        [XmlEnum("superset")]
        Superset
    }
}
