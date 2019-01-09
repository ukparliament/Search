// MIT License
//
// Copyright (c) 2019 UK Parliament
//
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

namespace SearchTests
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Text.RegularExpressions;
    using System.Xml;
    using System.Xml.Schema;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Search;

    [TestClass]
    public class RulesTests
    {
        public static IEnumerable<object[]> Patterns => Resources.Rules.SelectMany(rule => rule.Value).Select(pattern => new[] { pattern });

        [TestMethod]
        public void Rules_file_is_valid()
        {
            using (var schemaStream = Resources.GetStream("Search.Resources.Rules.xsd"))
            {
                using (var reader = XmlReader.Create(schemaStream))
                {
                    var settings = new XmlReaderSettings();
                    settings.Schemas.Add(string.Empty, reader);
                    settings.ValidationType = ValidationType.Schema;
                    settings.ValidationEventHandler += (object sender, ValidationEventArgs e) => throw e.Exception;

                    using (var rulesStream = Resources.GetStream("Search.Resources.Rules.xml"))
                    {
                        using (var rulesReader = XmlReader.Create(rulesStream, settings))
                        {
                            while (rulesReader.Read())
                            { }
                        }
                    }
                }
            }
        }

        [TestMethod]
        [DynamicData(nameof(Patterns))]
        public void Rule_pattern_is_valid_regex(string pattern)
        {
            new Regex(pattern);
        }
    }
}
