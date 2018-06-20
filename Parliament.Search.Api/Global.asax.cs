namespace Parliament.Search.Api
{
    using Microsoft.ApplicationInsights.Extensibility;
    using Parliament.OpenSearch;
    using Parliament.ServiceModel.Syndication;
    using System;
    using System.Configuration;
    using System.Web.Http;
    using System.Web.Http.ExceptionHandling;

    public class Global : System.Web.HttpApplication
    {
        protected void Application_Start(object sender, EventArgs e)
        {
            TelemetryConfiguration.Active.InstrumentationKey = ConfigurationManager.AppSettings["ApplicationInsightsInstrumentationKey"];

            var config = GlobalConfiguration.Configuration;

            config.Formatters.Add(new FeedFormatter());
            config.Formatters.Add(new DescriptionFormatter());

            config.Routes.MapHttpRoute("OpenApiDefinition", "openapi.json", new { controller = "OpenApiDefinition" });
            config.Routes.MapHttpRoute("NamedController", "{controller}");

            config.Services.Add(typeof(IExceptionLogger), new AIExceptionLogger());

            config.DependencyResolver = new DependencyResolver();

            config.Filters.Add(new RequestIdHeaderFilter());
            
        }
    }
}