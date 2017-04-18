using Microsoft.ApplicationInsights.Extensibility;
using Parliament.OpenSearch;
using Parliament.ServiceModel.Syndication;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Security;
using System.Web.SessionState;

namespace WebApplication1
{
    public class Global : System.Web.HttpApplication
    {

        protected void Application_Start(object sender, EventArgs e)
        {
            TelemetryConfiguration.Active.InstrumentationKey = ConfigurationManager.AppSettings["ApplicationInsightsInstrumentationKey"];

            var config = GlobalConfiguration.Configuration;

            //config.Formatters.Clear();
            config.Formatters.Add(new FeedFormatter());
            config.Formatters.Add(new DescriptionFormatter());

            config.Routes.MapHttpRoute("NamedController", "{controller}");
        }
    }
}