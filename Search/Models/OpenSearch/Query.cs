namespace OpenSearch
{
    using Newtonsoft.Json;
    using System;
    using System.Globalization;
    using System.Xml.Serialization;

    /// <summary>
    /// Describes a specific search request that can be made by the search client.
    /// </summary>
    [XmlRoot("Query", Namespace = Constants.NamespaceUri)]
    public class Query
    {
        private string title;
        private int? totalResults;
        private string searchTerms;
        private int? count;
        private int? startIndex;
        private int? startPage;
        private CultureInfo language;

        // TODO: implement extensibility
        /// <summary>
        /// Contains a string identifying how the search client should interpret the search request defined by this Query element.
        /// </summary>
        [XmlAttribute("role")]
        [JsonProperty("role")]
        public string Role { get; set; }

        /// <summary>
        /// Contains a human-readable plain text string describing the search request.
        /// </summary>
        [XmlAttribute("title")]
        [JsonProperty("title")]
        public string Title
        {
            get
            {
                return this.title;
            }
            set
            {
                if (value != null)
                {
                    Validation.ValidateLength(value, 256);
                    Validation.ValidatePlainText(value);
                }

                this.title = value;
            }
        }

        /// <summary>
        /// Contains the expected number of results to be found if the search request were made.
        /// </summary>
        [XmlIgnore]
        [JsonIgnore]
        public int? TotalResults
        {
            get
            {
                return this.totalResults;
            }
            set
            {
                if (value != null)
                {
                    Validation.ValidateNonNegative(value.Value);
                }

                this.totalResults = value;
            }
        }

        /// <summary>
        /// Contains the value representing the "searchTerms" as an OpenSearch 1.1 parameter.
        /// </summary>
        [XmlAttribute("searchTerms")]
        [JsonProperty("searchTerms")]
        public string SearchTerms
        {
            get
            {
                return this.searchTerms;
            }
            set
            {
                if (value != null)
                {
                    Validation.ValidateUrlEncoded(value);
                }

                this.searchTerms = value;
            }
        }

        /// <summary>
        /// Contains the value representing the "count" as a OpenSearch 1.1 parameter.
        /// </summary>
        [XmlIgnore]
        [JsonIgnore]
        public int? Count
        {
            get
            {
                return this.count;
            }
            set
            {
                if (value != null)
                {
                    Validation.ValidateNonNegative(value.Value);
                }

                this.count = value;
            }
        }

        /// <summary>
        /// Contains the value representing the "startIndex" as an OpenSearch 1.1 parameter.
        /// </summary>
        [XmlIgnore]
        [JsonIgnore]
        public int? StartIndex
        {
            get
            {
                return this.startIndex;
            }
            set
            {
                if (value != null)
                {
                    Validation.ValidateNonNegative(value.Value);
                }

                this.startIndex = value;
            }
        }

        // TODO: Default: The value specified by the "pageOffset" attribute of the containing Url element.
        /// <summary>
        /// Contains the value representing the "startPage" as an OpenSearch 1.1 parameter.
        /// </summary>
        [XmlIgnore]
        [JsonIgnore]
        public int? StartPage
        {
            get
            {
                return this.startPage;
            }
            set
            {
                this.startPage = value;
            }
        }

        /// <summary>
        /// Contains the value representing the "language" as an OpenSearch 1.1 parameter.
        /// </summary>
        /// <remarks>An OpenSearch description document should include one "Language" element for each language that the search engine supports. If the search engine also supports queries for any arbitrary language then the OpenSearch description document should include a Language element with a value of "*". The "language" template parameter in the OpenSearch URL template can be used to allow the search client to choose among the available languages.</remarks>
        [XmlIgnore]
        [JsonIgnore]
        public CultureInfo Language
        {
            get
            {
                return this.language;
            }
            set
            {
                this.language = value;
            }
        }

        #region Serialization overrides

        [XmlAttribute("totalResults")]
        [JsonProperty("totalResults")]
        [Obsolete("Used for serialization only")]
        public int SerializationTotalResults
        {
            get
            {
                return this.TotalResults.Value;
            }
            set
            {
                this.TotalResults = value;
            }
        }

        [XmlAttribute("count")]
        [JsonProperty("count")]
        [Obsolete("Used for serialization only")]
        public int SerializationCount
        {
            get
            {
                return this.Count.Value;
            }
            set
            {
                this.Count = value;
            }
        }

        [XmlAttribute("startIndex")]
        [JsonProperty("startIndex")]
        [Obsolete("Used for serialization only")]
        public int SerializationStartIndex
        {
            get
            {
                return this.StartIndex.Value;
            }
            set
            {
                this.StartIndex = value;
            }
        }

        [XmlAttribute("startPage")]
        [JsonProperty("startPage")]
        [Obsolete("Used for serialization only")]
        public int SerializationStartPage
        {
            get
            {
                return this.StartPage.Value;
            }
            set
            {
                this.StartPage = value;
            }
        }

        [XmlElement("language")]
        [JsonProperty("language")]
        [Obsolete("Used for serialization only")]
        public string SerializationLanguages
        {
            get
            {
                if (this.Language == null)
                {
                    return "*";
                }

                return this.Language.Name;
            }
            set
            {
                Validation.ValidateLanguage(value);

                this.Language = CultureInfo.CreateSpecificCulture(value);
            }
        }

        #endregion

        #region Serialization defaults

        public bool ShouldSerializeTitle()
        {
            return this.Title != null;
        }

        public bool ShouldSerializeSerializationTotalResults()
        {
            return this.TotalResults != null;
        }

        public bool ShouldSerializeSearchTerms()
        {
            return this.SearchTerms != null;
        }

        public bool ShouldSerializeSerializationCount()
        {
            return this.Count != null;
        }

        public bool ShouldSerializeSerializationStartIndex()
        {
            return this.StartIndex != null;
        }

        public bool ShouldSerializeSerializationStartPage()
        {
            return this.StartPage != null;
        }

        public bool ShouldSerializeSerializationLanguages()
        {
            return this.Language != null;
        }

        #endregion
    }
}
