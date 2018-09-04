namespace Parliament.Search.Api.Controllers
{
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Web.Http;

    public class HomeController : ApiController
    {
        public HttpResponseMessage Get()
        {
            var response = Request.CreateResponse();

            response.Content = new StringContent($@"<!DOCTYPE html>
<html>
    <head profile=""http://a9.com/-/spec/opensearch/1.1/"">
        <link rel=""search"" type=""application/opensearchdescription+xml"" href=""{Url.Route("Description", null)}"" title=""UK Parliament"" />
         <style>
            * {{
                font-style: normal;
                font-family: sans-serif;
                padding: 0;
                margin: 0;
            }}
          
            body {{
                max-width: 600px;
                padding: 20px;
            }}

            form {{
                margin-top: 20px;
            }}
        </style>
   </head>
    <body>
        <h1>parliament.uk search</h1>
        <form action=""{this.Url.Link("QueryWithExtension", new { ext = "html" })}"" method=""get"">
            <label for=""q"">search term</label>
            <input id=""q"" name=""q"">
            <input type=""submit"">
        </form>
    </body>
</html>", System.Text.Encoding.UTF8, "text/html");
            response.Content.Headers.ContentType = new MediaTypeHeaderValue("text/html");

            return response;
        }
    }
}