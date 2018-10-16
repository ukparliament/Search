namespace Search
{
    using System.Linq;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Routing;

    internal class ExtensionConstraint : IRouteConstraint
    {
        public bool Match(HttpContext httpContext, IRouter route, string routeKey, RouteValueDictionary values, RouteDirection routeDirection)
        {
            var extension = values[routeKey] as string;

            return Configuration.Mappings.Any(mapping => mapping.Extension == extension);
        }
    }
}
