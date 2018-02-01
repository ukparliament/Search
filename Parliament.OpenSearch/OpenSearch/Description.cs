namespace Parliament.Search.OpenSearch
{
    using Parliament.OpenSearch.Properties;
    using System;
    using System.Collections.ObjectModel;
    using System.Globalization;
    using System.Linq;
    using System.Net.Mail;
    using System.Text;
    using System.Xml;
    using System.Xml.Serialization;

    /// <summary>
    /// The root node of the OpenSearch description document.
    /// </summary>
    [XmlRoot("OpenSearchDescription", Namespace = Constants.NamespaceUri)]
    public class Description
    {
        //[XmlNamespaceDeclarations]
        //public static XmlSerializerNamespaces Namespaces
        //{
        //    get
        //    {
        //        return new XmlSerializerNamespaces(new XmlQualifiedName[] {
        //            new XmlQualifiedName(
        //                string.Empty,
        //                Constants.NamespaceUri)
        //        });
        //    }
        //}

        #region Data
        private string shortName;
        private string descriptionText;
        private Collection<Url> urls;
        private string contact;
        private Collection<string> tags;
        private string longName;
        private Collection<Image> images;
        private Collection<Query> queries;
        private string developer;
        private string attribution;
        private Collection<CultureInfo> languages;
        private Collection<Encoding> inputEncodings;
        private Collection<Encoding> outputEncodings;
        #endregion

        #region Elements

        /// <summary>
        /// Contains a brief human-readable title that identifies this search engine.
        /// </summary>
        public string ShortName
        {
            get
            {
                return Description.RequiredElementValue(this.shortName);
            }
            set
            {
                Validation.ValidateLength(value, 16);
                Validation.ValidatePlainText(value);
                Description.ValidateRequired(value);

                this.shortName = value;
            }
        }

        /// <summary>
        /// Contains a human-readable text description of the search engine.
        /// </summary>
        [XmlElement("Description")]
        public string DescriptionText
        {
            get
            {
                return Description.RequiredElementValue(this.descriptionText);
            }
            set
            {
                Validation.ValidateLength(value, 1024);
                Validation.ValidatePlainText(value);
                Description.ValidateRequired(value);

                this.descriptionText = value;
            }
        }

        /// <summary>
        /// Describes an interface by which a client can make requests for an external resource, such as search results, search suggestions, or additional description documents.
        /// </summary>
        [XmlElement("Url")]
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

        /// <summary>
        /// Contains an email address at which the maintainer of the description document can be reached.
        /// </summary>
        public string Contact
        {
            get
            {
                return this.contact;
            }
            set
            {
                Description.ValidateEmail(value);

                this.contact = value;
            }
        }

        // TODO: Limit length
        /// <summary>
        /// Contains a set of words that are used as keywords to identify and categorize this search content.
        /// </summary>
        [XmlIgnore]
        public Collection<string> Tags
        {
            get
            {
                if (this.tags == null)
                {
                    this.tags = new Collection<string>();
                }

                return this.tags;
            }
        }

        /// <summary>
        /// Contains an extended human-readable title that identifies this search engine.
        /// </summary>
        /// <remarks>Search clients should use the value of the <see cref="ShortName"/> element if this element is not available.</remarks>
        public string LongName
        {
            get
            {
                return this.longName;
            }
            set
            {
                Validation.ValidateLength(value, 48);
                Validation.ValidatePlainText(value);

                this.longName = value;
            }
        }

        /// <summary>
        /// Contains a URL that identifies the location of an image that can be used in association with this search content.
        /// </summary>
        /// <remarks>Image sizes are offered as a hint to the search client. The search client will choose the most appropriate image for the available space and should give preference to those listed first in the OpenSearch description document. Square aspect ratios are recommended. When possible, search engines should offer a 16x16 image of type "image/x-icon" or "image/vnd.microsoft.icon" (the Microsoft ICON format) and a 64x64 image of type "image/jpeg" or "image/png".</remarks>
        [XmlElement("Image")]
        public Collection<Image> Images
        {
            get
            {
                if (this.images == null)
                {
                    this.images = new Collection<Image>();
                }

                return this.images;
            }
        }

        /// <summary>
        /// Defines a search query that can be performed by search clients. Please see the OpenSearch Query element specification for more information.
        /// </summary>
        /// <remarks>OpenSearch description documents should include at least one Query element of role="example" that is expected to return search results. Search clients may use this example query to validate that the search engine is working properly.</remarks>
        [XmlElement("Query")]
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

        /// <summary>
        /// Contains the human-readable name or identifier of the creator or maintainer of the description document.
        /// </summary>
        /// <remarks>The developer is the person or entity that created the description document, and may or may not be the owner, author, or copyright holder of the source of the content itself.</remarks>
        public string Developer
        {
            get
            {
                return this.developer;
            }
            set
            {
                Validation.ValidateLength(value, 64);
                Validation.ValidatePlainText(value);

                this.developer = value;
            }
        }

        /// <summary>
        /// Contains a list of all sources or entities that should be credited for the content contained in the search feed.
        /// </summary>
        public string Attribution
        {
            get
            {
                return this.attribution;
            }
            set
            {
                Validation.ValidateLength(value, 256);
                Validation.ValidatePlainText(value);

                this.attribution = value;
            }
        }

        /// <summary>
        /// Contains a value that indicates the degree to which the search results provided by this search engine can be queried, displayed, and redistributed.
        /// </summary>
        public SyndicationRight SyndicationRight { get; set; }

        /// <summary>
        /// Contains a boolean value that should be set to true if the search results may contain material intended only for adults.
        /// </summary>
        /// <remarks>As there are no universally applicable guidelines as to what constitutes "adult" content, the search engine should make a good faith effort to indicate when there is a possibility that search results may contain material inappropriate for all audiences.</remarks>
        public bool AdultContent { get; set; }

        /// <summary>
        /// Contains a string that indicates that the search engine supports search results in the specified language.
        /// </summary>
        /// <remarks>An OpenSearch description document should include one "Language" element for each language that the search engine supports. If the search engine also supports queries for any arbitrary language then the OpenSearch description document should include a Language element with a value of "*". The "language" template parameter in the OpenSearch URL template can be used to allow the search client to choose among the available languages.</remarks>
        [XmlIgnore]
        public Collection<CultureInfo> Languages
        {
            get
            {
                if (this.languages == null)
                {
                    this.languages = new Collection<CultureInfo>();
                }

                return this.languages;
            }
        }

        /// <summary>
        /// Contains a string that indicates that the search engine supports search requests encoded with the specified character encoding.
        /// </summary>
        /// <remarks>An OpenSearch description document should include one "InputEncoding" element for each character encoding that can be used to encode search requests. The "inputEncoding" template parameter in the OpenSearch URL template can be used to require the search client to identify which encoding is being used to encode the current search request.</remarks>
        [XmlIgnore]
        public Collection<Encoding> InputEncodings
        {
            get
            {
                if (this.inputEncodings == null)
                {
                    this.inputEncodings = new Collection<Encoding>();
                }

                return this.inputEncodings;
            }
        }

        /// <summary>
        /// Contains a string that indicates that the search engine supports search responses encoded with the specified character encoding.
        /// </summary>
        /// <remarks>An OpenSearch description document should include one "OutputEncoding" element for each character encoding that can be used to encode search responses. The "outputEncoding" template parameter in the OpenSearch URL template can be used to allow the search client to choose a character encoding in the search response.</remarks>
        [XmlIgnore]
        public Collection<Encoding> OutputEncodings
        {
            get
            {
                if (this.outputEncodings == null)
                {
                    this.outputEncodings = new Collection<Encoding>();
                }

                return this.outputEncodings;
            }
        }

        #region Serialization overrides

        // TODO: setter
        [XmlElement("Tags")]
        [Obsolete("Used for serialization only")]
        public string SerializationTags
        {
            get
            {
                return string.Join(" ", this.Tags);
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        // TODO: this pattern doesn't work for deserialization
        [XmlElement("Language")]
        [Obsolete("Used for serialization only")]
        public Collection<string> SerializationLanguages
        {
            get
            {
                var query =
                    from current in this.Languages
                    let name = current == CultureInfo.InvariantCulture ? "*" : current.Name
                    select name;

                return new Collection<string>(query.ToList());
            }
        }

        [XmlElement("InputEncoding")]
        [Obsolete("Used for serialization only")]
        public Collection<string> SerializationInputEncodings
        {
            get
            {
                var query =
                    from encoding in this.InputEncodings
                    select encoding.WebName;

                return new Collection<string>(query.ToList());
            }
        }

        // TODO: implement setter?
        [XmlElement("OutputEncoding")]
        [Obsolete("Used for serialization only")]
        public Collection<string> SerializationOutputEncodings
        {
            get
            {
                var encodings =
                    from encoding in this.OutputEncodings
                    select encoding.WebName;

                return new Collection<string>(encodings.ToList());
            }
        }

        #endregion

        #endregion

        #region Serialization defaults

        [Obsolete("Used for serialization only")]
        public bool ShouldSerializeSerializationTags()
        {
            return this.Tags.Any();
        }

        [Obsolete("Used for serialization only")]
        public bool ShouldSerializeSyndicationRight()
        {
            return this.SyndicationRight != SyndicationRight.Open;
        }

        [Obsolete("Used for serialization only")]
        public bool ShouldSerializeAdultContent()
        {
            return this.AdultContent;
        }

        [Obsolete("Used for serialization only")]
        public bool ShouldSerializeSerializationLanguages()
        {
            return this.Languages.Any(language => language != CultureInfo.InvariantCulture);
        }

        [Obsolete("Used for serialization only")]
        public bool ShouldSerializeSerializationInputEncodings()
        {
            return this.InputEncodings.Any(encoding => encoding != Encoding.UTF8);
        }

        [Obsolete("Used for serialization only")]
        public bool ShouldSerializeSerializationOutputEncodings()
        {
            return this.OutputEncodings.Any(encoding => encoding != Encoding.UTF8);
        }

        #endregion

        #region Validation

        private static void ValidateRequired(string value)
        {
            if (value == null)
            {
                throw new ArgumentNullException("value", Resources.ElementRequired);
            }
        }

        private static void ValidateEmail(string value)
        {
            try
            {
                new MailAddress(value).ToString();
            }
            catch (Exception e)
            {
                throw new FormatException(Resources.EmailInvalid, e);
            }
        }

        #endregion

        private static string RequiredElementValue(string value)
        {
            if (value == null)
            {
                return string.Empty;
            }

            return value;
        }
    }
}
