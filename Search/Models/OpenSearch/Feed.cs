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
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.ServiceModel.Syndication;
    using System.Xml;
    using System.Xml.Serialization;
    using Newtonsoft.Json;

    public class Feed : SyndicationFeed
    {
        public Collection<Query> Queries { get; } = new Collection<Query>();

        public int TotalResults { get; set; }

        public int StartIndex { get; set; }

        public int ItemsPerPage { get; set; }

        protected override bool TryParseElement(XmlReader reader, string version)
        {
            if (reader.NamespaceURI == Constants.NamespaceUri)
            {
                var mappings = new Dictionary<string, (Type Type, Action<object> Action)>() {
                        { "Query", (typeof(Query), item => this.Queries.Add(item as Query)) },
                        { "totalResults", (typeof(TotalResults), item => this.TotalResults = (item as TotalResults).Value) },
                        { "startIndex", (typeof(StartIndex), item => this.StartIndex = (item as StartIndex).Value) },
                        { "itemsPerPage", (typeof(ItemsPerPage), item => this.ItemsPerPage = (item as ItemsPerPage).Value) }
                    };

                if (mappings.ContainsKey(reader.LocalName))
                {
                    var mapping = mappings[reader.LocalName];
                    var serializer = new XmlSerializer(mapping.Type);
                    var instance = serializer.Deserialize(reader);
                    mapping.Action(instance);

                    return true;
                }
            }

            return base.TryParseElement(reader, version);
        }

        protected override void WriteElementExtensions(XmlWriter writer, string version)
        {
            var mappings = new (Type Type, Action<XmlSerializer> Action)[] {
                (typeof(Query), serializer => { foreach (var query in this.Queries) { serializer.Serialize(writer, query, Constants.Namespaces); } }),
                (typeof(TotalResults), serializer => serializer.Serialize(writer, new TotalResults { Value = this.TotalResults }, Constants.Namespaces)),
                (typeof(StartIndex), serializer => serializer.Serialize(writer, new StartIndex { Value = this.StartIndex }, Constants.Namespaces)),
                (typeof(ItemsPerPage), serializer => serializer.Serialize(writer, new ItemsPerPage { Value = this.ItemsPerPage }, Constants.Namespaces))
            };

            foreach (var mapping in mappings)
            {
                mapping.Action(new XmlSerializer(mapping.Type));
            }

            base.WriteElementExtensions(writer, version);
        }
    }
}
