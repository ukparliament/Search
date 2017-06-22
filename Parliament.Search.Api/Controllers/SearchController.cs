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

        public Feed Get([FromUri(Name = "q")]string searchTerms, [FromUri(Name = "start")]int startIndex = 1, [FromUri(Name = "count")]int count = 10)
        {
            if (searchTerms == null)
            {
                throw new ArgumentNullException("q", "Search terms must be specified. ");
            }
            if (startIndex < 1)
            {
                throw new ArgumentOutOfRangeException("start ", startIndex, "Allowed values are >=1. ");
            }
            if (count < 1 | count > 100)
            {
                throw new ArgumentOutOfRangeException("count ", count, "Allowed values are 1-100 inclusive. ");
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
