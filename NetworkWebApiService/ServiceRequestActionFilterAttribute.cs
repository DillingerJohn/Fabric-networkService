using System;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using NetworkService.Core.Logging;
//using log4net;

namespace NetworkWebApiService
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = true, AllowMultiple = false)]
    internal sealed class ServiceRequestActionFilterAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(HttpActionContext actionContext)
        {
           // NetworkService.Core.Logging.Logger.Instanse.Log("Info", "http request: " + actionContext.ActionDescriptor.ActionName);
            ServiceEventSource.Current.ServiceRequestStart(actionContext.ActionDescriptor.ActionName);
        }

        public override void OnActionExecuted(HttpActionExecutedContext actionExecutedContext)
        {
           // NetworkService.Core.Logging.Logger.Instanse.Log("Info", "http response: " + actionExecutedContext.ActionContext.ActionDescriptor.ActionName);
            ServiceEventSource.Current.ServiceRequestStop(actionExecutedContext.ActionContext.ActionDescriptor.ActionName,
                actionExecutedContext.Exception?.ToString() ?? string.Empty);
        }
    }
}
