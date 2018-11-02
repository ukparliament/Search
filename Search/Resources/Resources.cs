namespace Search
{
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Xml.Linq;
    using Microsoft.OpenApi.Any;
    using Microsoft.OpenApi.Exceptions;
    using Microsoft.OpenApi.Models;
    using Microsoft.OpenApi.Readers;

    public static class Resources
    {
        public static Dictionary<string, IEnumerable<string>> Rules
        {
            get
            {
                using (var stream = Resources.GetStream("Search.Resources.Rules.xml"))
                {
                    return XDocument.Load(stream).Descendants("Rule").ToDictionary(el => el.Element("Label").Value, el => el.Elements("Pattern").Select(pattern => pattern.Value));
                }
            }
        }

        public static OpenApiDocument OpenApiDocument
        {
            get
            {
                using (var stream = Resources.GetStream("Search.Resources.OpenApiDocumentTemplate.json"))
                {
                    var reader = new OpenApiStreamReader();
                    var document = reader.Read(stream, out var diagnostic);

                    if (diagnostic.Errors.Any())
                    {
                        throw new OpenApiException(diagnostic.Errors.First().Message);
                    }

                    document.Components.Responses["searchResponse"].Content = Configuration.QueryMappings.ToDictionary(m => m.MediaType, m => new OpenApiMediaType());
                    document.Paths["/query.{extension}"].Operations[OperationType.Get].Parameters[0].Schema.Enum = Configuration.QueryMappings.Select(m => new OpenApiString(m.Extension) as IOpenApiAny).ToList();

                    return document;
                }
            }
        }

        public static Stream GetStream(string resourceName)
        {
            return Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceName);
        }
    }
}
