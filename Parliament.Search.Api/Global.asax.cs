namespace Parliament.Search.Api
{
    using System;
    using System.Configuration;
    using System.Web;
    using System.Web.Http;
    using Microsoft.ApplicationInsights.Extensibility;

    public class Global : HttpApplication
    {
        protected void Application_Start(object sender, EventArgs e)
        {
            TelemetryConfiguration.Active.InstrumentationKey = ConfigurationManager.AppSettings["ApplicationInsightsInstrumentationKey"];

            var config = GlobalConfiguration.Configuration;

            config.Formatters.Clear();
            config.Formatters.Add(new HttpErrorJsonFormatter());

            config.Routes.MapHttpRoute("Home", "", new { controller = "Home" });
            config.Routes.MapHttpRoute("QueryWithExtension", "query.{ext}", new { controller = "Query" });
            config.Routes.MapHttpRoute("Query", "query", new { controller = "Query", ext = string.Empty });
            config.Routes.MapHttpRoute("Description", "description", new { controller = "Description" });
            config.Routes.MapHttpRoute("OpenApiDefinition", "openapi.json", new { controller = "OpenApiDefinition" });
            config.Routes.MapHttpRoute("BadRequest", "{*any}", new { controller = "BadRequest" });

            config.DependencyResolver = new DependencyResolver();

            config.Filters.Add(new RequestIdHeaderFilter());

        }
    }
}