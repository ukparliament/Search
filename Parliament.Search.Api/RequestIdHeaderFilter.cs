using System.Web;
using System.Web.Http.Filters;

namespace Parliament.Search.Api
{
    public class RequestIdHeaderFilter : ActionFilterAttribute
    {
        public override void OnActionExecuted(HttpActionExecutedContext actionExecutedContext)
        {
            string operationId = HttpContext.Current.GetRequestTelemetry().Context.Operation.ParentId;
            if (string.IsNullOrWhiteSpace(operationId) == false)
            {
                actionExecutedContext.Response.Headers.Add("Request-Id", operationId);
            }
        }
    }
}