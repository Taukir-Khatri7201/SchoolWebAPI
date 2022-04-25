using SchoolWebAPI.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using Unity;
using Unity.WebApi;

namespace SchoolWebAPI.Filters
{
    public class CustomLoggerActionFilterAttribute : ActionFilterAttribute
    {
        private readonly ILogger logger;

        public CustomLoggerActionFilterAttribute()
        {
            logger = UnityConfig.container.Resolve<ILogger>();
        }

        public override void OnActionExecuting(HttpActionContext actionContext)
        {
            logger.Info("Entered " + actionContext.ActionDescriptor.ActionName + " at " + DateTime.Now.ToString());
        }

        public override void OnActionExecuted(HttpActionExecutedContext actionExecutedContext)
        {
            logger.Info("Left " + actionExecutedContext.ActionContext.ActionDescriptor.ActionName + " at " + DateTime.Now.ToString());
        }
    }
}