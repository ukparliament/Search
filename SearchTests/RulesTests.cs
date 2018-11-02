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
