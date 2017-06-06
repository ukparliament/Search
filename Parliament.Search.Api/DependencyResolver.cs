namespace Parliament.Search.Api
{
    using Parliament.Search.Api.Controllers;
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Linq;
    using System.Web.Http.Dependencies;

    internal class DependencyResolver : IDependencyResolver
    {
        public IDependencyScope BeginScope() => this;

        public void Dispose() { }

        public object GetService(Type serviceType)
        {
            if (DependencyResolver.UseMockEngine)
            {
                if (serviceType == typeof(SearchController))
                {
                    return new SearchController(new MockEngine());
                }
            }

            return null;
        }

        public IEnumerable<object> GetServices(Type serviceType)
        {
            return Enumerable.Empty<object>();
        }

        private static bool UseMockEngine
        {
            get
            {
                bool.TryParse(ConfigurationManager.AppSettings["UseMockEngine"], out bool result);

                return result;
            }
        }
    }
}