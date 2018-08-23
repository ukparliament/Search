namespace Parliament.Search.Api.Controllers
{
    using System.Net;
    using System.Net.Http;
    using System.Web.Http;

    public class OpenApiDefinitionController : ApiController
    {
        public HttpResponseMessage Get()
        {
            var response = this.Request.CreateResponse(HttpStatusCode.OK);
            response.Content = new StringContent(Resources.OpenApiDocument, System.Text.Encoding.UTF8, "application/json");

            return response;
        }
    }
}
