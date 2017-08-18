namespace Parliament.Search.Api.Controllers
{
    using Library;
    using Microsoft.ApplicationInsights;
    using OpenSearch;
    using System;
    using System.Collections.Generic;
    using System.Web.Http;

    public class SearchController : ApiController
    {
        private readonly IEngine engine;

        public SearchController(IEngine engine)
        {
            this.engine = engine;
        }

        public dynamic Get([FromUri(Name = "q")]string searchTerms, [FromUri(Name = "start")]int startIndex = 1, [FromUri(Name = "count")]int count = 10)
        {
            if (searchTerms == null)
            {
                return BadRequest("The q query string parameter must be specified");
            }
            if (startIndex < 1)
            {
                return BadRequest("The startIndex query string parameter must be > 1");
            }
            if (count < 1 | count > 100)
            {
                return BadRequest("The count query string parameter must be > 1 and < 100");
            }

            Feed response = null;

            var telemetry = new TelemetryClient();
            var timer = System.Diagnostics.Stopwatch.StartNew();
            var startTime = DateTime.UtcNow;
            try
            {
                response = engine.Search(searchTerms, startIndex, count);
            }
            finally
            {
                timer.Stop();
                telemetry.TrackDependency(string.Format("Search Engine: {0}", engine.GetType().FullName), searchTerms, startTime, timer.Elapsed, response != null);
            }

            telemetry.TrackMetric("TotalResults", response.TotalResults, new Dictionary<string, string> {
                { "searchTerms", searchTerms },
                { "startIndex", startIndex.ToString() },
                { "count", count.ToString() }
            });

            return response;
        }
    }
}
