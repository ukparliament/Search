namespace Parliament.Search.Api
{
    using BingProvider;
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
            if (serviceType == typeof(QueryController))
            {
                bool.TryParse(ConfigurationManager.AppSettings["UseMockEngine"], out bool useMockEngine);
                if (useMockEngine)
                {
                    return new QueryController(new MockEngine());
                }

                bool.TryParse(ConfigurationManager.AppSettings["UseBingEngine"], out bool useBingEngine);
                if (useBingEngine)
                {
                    return new QueryController(new BingEngine());
                }
            }

            return null;
        }

        public IEnumerable<object> GetServices(Type serviceType)
        {
            return Enumerable.Empty<object>();
        }
    }
}