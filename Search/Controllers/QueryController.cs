namespace Search
{
    using System;
    using System.Linq;
    using System.ServiceModel.Syndication;
    using System.Text.RegularExpressions;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Formatters;
    using Microsoft.AspNetCore.Mvc.Infrastructure;
    using Microsoft.Extensions.Options;
    using Microsoft.Net.Http.Headers;
    using OpenSearch;

    public class QueryController : Controller
    {
        private readonly IEngine engine;
        private readonly MvcOptions options;
        private readonly OutputFormatterSelector selector;

        public QueryController(IEngine engine, IOptions<MvcOptions> options, OutputFormatterSelector selector)
        {
            this.engine = engine;
            this.options = options.Value;
            this.selector = selector;
        }

        [HttpGet("query")]
        [HttpGet("query.{format:query}", Name = "QueryWithExtension")]
        [FormatFilter]
        public async Task<ActionResult> Get(QueryParameters parameters)
        {
            var responseFeed = await this.engine.Query(parameters.SearchTerms, parameters.StartIndex, parameters.Count);

            this.ProcessFeed(responseFeed);

            switch (this.GetResponseContentType(parameters))
            {
                case "text/html":
                    return this.View(responseFeed);

                default:
                    return this.Ok(responseFeed);
            }
        }

        private string GetResponseContentType(QueryParameters parameters)
        {
            var mediaTypes = new MediaTypeCollection();
            if (!(parameters.Format is null))
            {
                mediaTypes.Add(
                    new MediaTypeHeaderValue(
                        this.options.FormatterMappings.GetMediaTypeMappingForFormat(
                            parameters.Format)));
            }

            var formatter = this.selector.SelectFormatter(
                new OutputFormatterWriteContext(
                    this.HttpContext,
                    (s, e) => null,
                    typeof(Feed),
                    null),
                this.options.OutputFormatters.OfType<FeedFormatter>().Cast<IOutputFormatter>().ToList(),
                mediaTypes);

            return ((FeedFormatter)formatter).SupportedMediaTypes.Single();
        }

        private void ProcessFeed(Feed responseFeed)
        {
            foreach (var item in responseFeed.Items)
            {
                this.ProcessItem(item);
            }
        }

        private void ProcessItem(SyndicationItem item)
        {
            var uri = item.Links.SingleOrDefault().Uri;
            var hintsExtension = this.ProcessUri(uri);

            item.ElementExtensions.Add(hintsExtension);

            //foreach (var hint in hintsExtension.Hints)
            //{
            //    this.telemetry.TrackEvent(
            //        "HintMatch",
            //        new Dictionary<string, string> {
            //            { "Hints", hint.Label },
            //            { "URL", uri.ToString() }
            //        },
            //        new Dictionary<string, double> {
            //            { "HintsMatchedPerItem", hintsExtension.Hints.Count() }
            //        }
            //    );
            //}
        }

        private HintsExtension ProcessUri(Uri uri)
        {
            return new HintsExtension(
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
}
