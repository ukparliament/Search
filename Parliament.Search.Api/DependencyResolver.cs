namespace Parliament.Search.Api
{
    using BingProvider;
    using GoogleProvider;
    using MockProvider;
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
            if (serviceType == typeof(SearchController))
            {
                bool.TryParse(ConfigurationManager.AppSettings["UseMockEngine"], out bool useMockEngine);
                if (useMockEngine)
                {
                    return new SearchController(new MockEngine());
                }

                bool.TryParse(ConfigurationManager.AppSettings["UseBingEngine"], out bool useBingEngine);
                if (useBingEngine)
                {
                    return new SearchController(new BingEngine());
                }

                return new SearchController(new GoogleEngine());
            }

            return null;
        }

        public IEnumerable<object> GetServices(Type serviceType)
        {
            return Enumerable.Empty<object>();
        }
    }
}