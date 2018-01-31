namespace Parliament.Search.Api.Controllers
{
    using Library;
    using Microsoft.ApplicationInsights;
    using Microsoft.ApplicationInsights.DataContracts;
    using Newtonsoft.Json;
    using OpenSearch;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.Http;
    using System.ServiceModel.Syndication;
    using System.Text.RegularExpressions;
    using System.Web.Http;
    using System.Xml.Linq;

    public class SearchController : ApiController
    {
        private readonly IEngine engine;

        private TelemetryClient telemetryClient = new TelemetryClient();
        
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

            responseFeed = engine.Search(searchTerms, startIndex, count);

            ProcessFeed(responseFeed);

            telemetryClient.Context.Properties["searchTerms"] = searchTerms;
            telemetryClient.Context.Properties["startIndex"] = startIndex.ToString();
            telemetryClient.Context.Properties["count"] = count.ToString();
            telemetryClient.TrackMetric(new MetricTelemetry("TotalResults", responseFeed.TotalResults));

            return ResponseMessage(Request.CreateResponse(responseFeed));
        }

        private void ProcessFeed(Feed responseFeed)
        {
            foreach (var item in responseFeed.Items)
            {
                ProcessItem(item);
            }
        }

        private void ProcessItem(SyndicationItem item)
        {
            var uri = item.Links.SingleOrDefault().Uri;
            var hintsExtension = ProcessUri(uri);

            item.ElementExtensions.Add(hintsExtension);

            foreach (Hint hint in hintsExtension.Hints)
            {
                telemetryClient.TrackEvent("HintMatch", new Dictionary<string, string> {
                { "Hints", hint.Label },
                { "URL", uri.ToString() }
            }, new Dictionary<string, double> { { "HintsMatchedPerItem", hintsExtension.Hints.Count() } });
            }
        }
    
        private HintsWrapper ProcessUri(Uri uri)
        {
            return new HintsWrapper(
                Resources
                .Rules
                .Select(rule => new { Rule = rule, Match = Regex.Match(uri.AbsoluteUri, rule.Key) })
                .Where(result => result.Match.Success)
                .Select(result => new Hint
                {
                    Label = string.Format(result.Rule.Value, result.Match.Groups.Cast<Group>().Select(group => group.Value).ToArray()),
                    Filter = result.Match.Groups["filter"].Value
                })
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
