namespace Parliament.Search.ApiTests
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Text.RegularExpressions;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Parliament.Search.Api;

    [TestClass]
    public class RulesTests
    {
        public static IEnumerable<object[]> Rules => Resources.Rules.Select(r => new[] { r.Key });

        [TestMethod]
        public void Rules_file_is_valid()
        {
            var rules = Resources.Rules;
        }

        [TestMethod]
        [DynamicData(nameof(Rules))]
        public void Rules_regex_is_valid(string rule)
        {
            var regex = new Regex(rule);
        }
    }
}
