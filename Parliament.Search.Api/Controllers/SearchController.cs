namespace Parliament.Search.Api.Controllers
{
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

        public Feed Get([FromUri(Name = "q")]string searchTerms)
        {
            return Get(searchTerms, 1);
        }

        public Feed Get([FromUri(Name = "q")]string searchTerms, [FromUri(Name = "start")]int startIndex)
        {
            return Get(searchTerms, startIndex, 10);
        }

        public Feed Get([FromUri(Name = "q")]string searchTerms, [FromUri(Name = "start")]int startIndex, [FromUri(Name = "pagesize")]int pageSize)
        {
            return this.engine.Search(searchTerms, startIndex, pageSize);
        }
    }
}
