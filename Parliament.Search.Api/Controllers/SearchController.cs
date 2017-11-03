namespace Parliament.Search.Api.Controllers
{
    using Library;
    using Microsoft.ApplicationInsights;
    using Newtonsoft.Json;
    using OpenSearch;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.Http;
    using System.ServiceModel.Syndication;
    using System.Web.Http;
    using System.Xml.Linq;

    public class SearchController : ApiController
    {
        private readonly IEngine engine;

        public SearchController(IEngine engine)
        {
            this.engine = engine;
        }

        public IHttpActionResult Get([FromUri(Name = "q")]string searchTerms, [FromUri(Name = "start")]int startIndex = 1, [FromUri(Name = "count")]int count = 10)
        {
            if (string.IsNullOrWhiteSpace(searchTerms))
            {
                return this.BadRequest("The q query string parameter must be specified");
            }
            if (startIndex < 1)
            {
                return BadRequest("The startIndex query string parameter must be > 1");
            }
            if (count < 1 | count > 100)
            {
                return BadRequest("The count query string parameter must be > 1 and < 100");
            }

            Feed responseFeed = null;

            var telemetry = new TelemetryClient();
            var timer = System.Diagnostics.Stopwatch.StartNew();
            var startTime = DateTime.UtcNow;
            try
            {
                responseFeed = engine.Search(searchTerms, startIndex, count);
            }
            finally
            {
                timer.Stop();
                telemetry.TrackDependency(string.Format("Search Engine: {0}", engine.GetType().FullName), searchTerms, startTime, timer.Elapsed, responseFeed != null);
            }

            SearchController.ProcessFeed(responseFeed);

            telemetry.TrackMetric("TotalResults", responseFeed.TotalResults, new Dictionary<string, string> {
                { "searchTerms", searchTerms },
                { "startIndex", startIndex.ToString() },
                { "count", count.ToString() }
            });

            return ResponseMessage(Request.CreateResponse(responseFeed));
        }

        private static void ProcessFeed(Feed responseFeed)
        {
            foreach (var item in responseFeed.Items)
            {
                SearchController.ProcessItem(item);
            }
        }

        private static void ProcessItem(SyndicationItem item)
        {
            var uri = item.BaseUri;
            var hintsExtension = SearchController.ProcessUri(uri);

            item.ElementExtensions.Add(hintsExtension);
        }

        private static object ProcessUri(Uri uri)
        {
            return new HintsWrapper(
                new[] {
                    new Hint {
                        Label = "hint1",
                        Filter = "filter1" },
                    new Hint {
                        Label = "hint2",
                        Filter = "filter2" }
                }
            );
        }
    }

    class HintsWrapper : SyndicationElementExtension
    {
        public HintsWrapper(IEnumerable<Hint> hints) : base(new XElement("hints"))
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

        private XElement XmlData
        {
            get
            {
                return this.GetObject<XElement>();
            }
        }
    }

    public class Hint
    {
        public string Label;
        public string Filter;
    }
}
