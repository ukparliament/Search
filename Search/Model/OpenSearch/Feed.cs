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
