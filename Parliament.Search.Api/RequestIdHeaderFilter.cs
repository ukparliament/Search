namespace Parliament.Search.Api
{
    using System.Web;
    using System.Web.Http.Filters;

    public class RequestIdHeaderFilter : ActionFilterAttribute
    {
        public override void OnActionExecuted(HttpActionExecutedContext actionExecutedContext)
        {
            var operationId = HttpContext.Current.GetRequestTelemetry().Context.Operation.ParentId;

            if (!string.IsNullOrWhiteSpace(operationId))
            {
                actionExecutedContext.Response.Headers.Add("Request-Id", operationId);
            }
        }
    }
}