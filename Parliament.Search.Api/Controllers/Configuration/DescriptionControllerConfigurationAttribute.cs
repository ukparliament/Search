namespace Parliament.Search.Api.Controllers
{
    using System;
    using System.Web.Http.Controllers;
    using Parliament.OpenSearch;

    [AttributeUsage(AttributeTargets.Class)]
    public sealed class DescriptionControllerConfigurationAttribute : Attribute, IControllerConfiguration
    {
        public void Initialize(HttpControllerSettings controllerSettings, HttpControllerDescriptor controllerDescriptor)
        {
            controllerSettings.Formatters.Add(new DescriptionFormatter());
        }
    }
}
