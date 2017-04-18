namespace Parliament.Search.Api.Controllers
{
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Web.Http;

    public class AutodiscoveryController : ApiController
    {
        public HttpResponseMessage Get()
        {
            var response = Request.CreateResponse();

            response.Content = new StringContent($@"<!DOCTYPE html>
<html>
    <head profile='http://a9.com/-/spec/opensearch/1.1/'>
        <link title='os' type='application/opensearchdescription+xml' rel='search' href='{Url.Route("NamedController", new { controller = "description" })}' />
    </head>
    <body>Hello World</body>
</html>");
            response.Content.Headers.ContentType = new MediaTypeHeaderValue("text/html");

            return response;
        }
    }
}