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
