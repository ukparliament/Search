// MIT License
//
// Copyright (c) 2019 UK Parliament
//
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

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
