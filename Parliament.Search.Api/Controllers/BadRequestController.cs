namespace Parliament.Search.Api.Controllers
{
    using System.Web.Http;

    [BadRequestControllerConfiguration]
    public class BadRequestController : ApiController
    {
        public IHttpActionResult Default()
        {
            return this.BadRequest();
        }
    }
}
