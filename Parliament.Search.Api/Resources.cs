namespace Parliament.Search.Api
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Reflection;
    using Microsoft.VisualBasic.FileIO;

    public static class Resources
    {
        public static Dictionary<string, string> Rules { get; } = Resources.GetRules();

        internal static string OpenApiDocument { get; } = Resources.GetString("Parliament.Search.Api.OpenApiDefinition.json");

        private static Dictionary<string, string> GetRules()
        {
            var rules = new Dictionary<string, string>();

            using (var stream = Resources.GetStream("Parliament.Search.Api.Rules.tsv"))
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

        internal static Stream GetStream(string resourceName)
        {
            return Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceName);
        }

        private static string GetString(string resourceName)
        {
            using (var resourceStream = Resources.GetStream(resourceName))
            {
                using (var reader = new StreamReader(resourceStream))
                {
                    return reader.ReadToEnd();
                }
            }
        }
    }
}