namespace Parliament.Search.Api.Controllers
{
    using Library;
    using OpenSearch;
    using System.Web.Http;

    public class SearchController : ApiController
    {
        private readonly IEngine engine;

        public SearchController() : this(new Engine()) { }

        public SearchController(IEngine engine)
        {
            this.engine = engine;
        }

        public Feed Get([FromUri(Name = "q")]string searchTerms, [FromUri(Name = "start")]int startIndex = 1, [FromUri(Name = "pagesize")]int pageSize = 10)
        {
            return this.engine.Search(searchTerms, startIndex, pageSize);
        }
    }
}
