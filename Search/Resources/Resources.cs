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
