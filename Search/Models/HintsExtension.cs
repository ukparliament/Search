// MIT License
//
// Copyright (c) 2019 UK Parliament
//
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

namespace Search
{
    using System.Collections.Generic;
    using System.Linq;
    using System.ServiceModel.Syndication;
    using System.Xml.Linq;
    using Newtonsoft.Json;

    public class HintsExtension : SyndicationElementExtension
    {
        public HintsExtension(IEnumerable<Hint> hints) : base(new XElement("hints"))
        {
            this.Hints = hints;
        }

        [JsonProperty("Contents")]
        public IEnumerable<Hint> Hints
        {
            get
            {
                var hintElements = this.XmlData.Elements("hint");

                return hintElements.Select(hintElement => new Hint
                {
                    Label = hintElement.Element("Name").Value,
                    Filter = hintElement.Element("Filter").Value
                });
            }
            private set
            {
                var hintElements = value.Select(hint => new XElement("hint", new[] {
                    new XElement("Name", hint.Label),
                    new XElement("Filter", hint.Filter) }));

                this.XmlData.Add(hintElements);
            }
        }

        private XElement XmlData => this.GetObject<XElement>();
    }
}
