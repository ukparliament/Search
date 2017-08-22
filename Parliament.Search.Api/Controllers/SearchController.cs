namespace Parliament.Search.Api.Controllers
{
    using Library;
    using Microsoft.ApplicationInsights;
    using OpenSearch;
    using System;
    using System.Collections.Generic;
    using System.Net.Http;
    using System.Web.Http;

    public class SearchController : ApiController
    {
        private readonly IEngine engine;

        public SearchController(IEngine engine)
        {
            this.engine = engine;
        }

        public IHttpActionResult Get([FromUri(Name = "q")]string searchTerms, [FromUri(Name = "start")]int startIndex = 1, [FromUri(Name = "pagesize")]int count = 10)
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
                return BadRequest("The pagesize query string parameter must be > 1 and < 100");
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

            telemetry.TrackMetric("TotalResults", responseFeed.TotalResults, new Dictionary<string, string> {
                { "searchTerms", searchTerms },
                { "startIndex", startIndex.ToString() },
                { "count", count.ToString() }
            });
            
            return ResponseMessage(Request.CreateResponse(responseFeed));
        }
    }
}
