namespace Parliament.Search.OpenSearch
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.ServiceModel.Syndication;
    using System.Xml;
    using System.Xml.Serialization;

    public class Feed : SyndicationFeed
    {
        private Collection<Url> urls;
        private Collection<Query> queries;
        private int totalResults;
        private int startIndex;

        public Collection<Query> Queries
        {
            get
            {
                if (this.queries == null)
                {
                    this.queries = new Collection<Query>();
                }

                return this.queries;
            }
        }

        public Collection<Url> Urls
        {
            get
            {
                if (this.urls == null)
                {
                    this.urls = new Collection<Url>();
                }

                return this.urls;
            }
        }

        public int TotalResults
        {
            get
            {
                return this.totalResults;
            }
            set
            {
                this.totalResults = value;
            }
        }

        public int StartIndex
        {
            get
            {
                return this.startIndex;
            }
            set
            {
                this.startIndex = value;
            }
        }

        protected override bool TryParseElement(XmlReader reader, string version)
        {
            if (reader.NamespaceURI == Constants.NamespaceUri)
            {
                var mappings = new Dictionary<string, Tuple<Type, Action<object>>>() {
                        { "Url", Tuple.Create<Type, Action<object>>(typeof(Url), item => this.Urls.Add(item as Url)) },
                        { "Query", Tuple.Create<Type, Action<object>>(typeof(Query), item => this.Queries.Add(item as Query)) },
                        { "totalResults", Tuple.Create<Type, Action<object>>(typeof(TotalResults), item => this.totalResults = (item as TotalResults).Value) },
                        { "startIndex", Tuple.Create<Type, Action<object>>(typeof(StartIndex), item => this.startIndex = (item as StartIndex).Value) }
                    };

                if (mappings.ContainsKey(reader.LocalName))
                {
                    var mapping = mappings[reader.LocalName];
                    var type = mapping.Item1;
                    var inserter = mapping.Item2;
                    var serializer = new XmlSerializer(type);
                    var instance = serializer.Deserialize(reader);
                    inserter(instance);

                    return true;
                }
            }

            return base.TryParseElement(reader, version);
        }

        protected override void WriteElementExtensions(XmlWriter writer, string version)
        {
            var mappings = new List<Tuple<Type, Action<XmlSerializer>>>() {
                    Tuple.Create<Type, Action<XmlSerializer>>(typeof(Url), serializer => { foreach (var url in this.Urls) {serializer.Serialize(writer, url,Constants.Namespaces);}}),
                    Tuple.Create<Type, Action<XmlSerializer>>(typeof(Query), serializer => { foreach (var query in this.Queries) {serializer.Serialize(writer, query,Constants.Namespaces);}}),
                    Tuple.Create<Type, Action<XmlSerializer>>(typeof(TotalResults), serializer => serializer.Serialize(writer, new TotalResults() {Value = this.TotalResults},Constants.Namespaces)),
                    Tuple.Create<Type, Action<XmlSerializer>>(typeof(StartIndex), serializer => serializer.Serialize(writer, new StartIndex() {Value = this.StartIndex},Constants.Namespaces))
                };

            foreach (var mapping in mappings)
            {
                var type = mapping.Item1;
                var serializer = new XmlSerializer(type);
                var writeAction = mapping.Item2;
                writeAction(serializer);
            }

            base.WriteElementExtensions(writer, version);
        }
    }
}
