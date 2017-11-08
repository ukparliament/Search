namespace Parliament.Search.Api
{
    using Microsoft.VisualBasic.FileIO;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Reflection;

    public class Resources
    {
        private static Dictionary<string, string> rules;

        public static Dictionary<string, string> Rules
        {
            get
            {
                if (Resources.rules == null)
                {
                    Resources.rules = GetRules();
                }

                return Resources.rules;
            }
        }

        private static Dictionary<string, string> GetRules()
        {
            var rules = new Dictionary<string, string>();

            using (var stream = GetStream("Parliament.Search.Api.Rules.tsv"))
            {
                using (var parser = new TextFieldParser(stream) { Delimiters = new[] { "\t" }, CommentTokens = new[] { "#" } })
                {
                    while (!parser.EndOfData)
                    {
                        var fields = parser.ReadFields();

                        if (fields.Length != 2)
                        {
                            throw new FormatException(string.Format("Every line must have two fields delimited by a TAB character. Line {0} had {1} field(s)", parser.LineNumber - 1, fields.Length));
                        }

                        rules[fields[0]] = fields[1];
                    }
                }
            }

            return rules;
        }

        private static Stream GetStream(string resourceName)
        {
            return Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceName);
        }
    }
}