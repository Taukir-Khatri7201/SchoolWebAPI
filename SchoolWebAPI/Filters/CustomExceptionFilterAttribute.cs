using System.Net;
using System.Net.Http;
using System.Web.Http.Filters;
using SchoolWebAPI.Repositories;
using DataAccess.Utility;
using Unity;

namespace SchoolWebAPI.Filters
{
    public class CustomExceptionFilterAttribute : ExceptionFilterAttribute
    {
        private readonly ILogger logger;

        public CustomExceptionFilterAttribute()
        {
            this.logger = UnityConfig.container.Resolve<ILogger>();
        }

        public override void OnException(HttpActionExecutedContext actionExecutedContext)
        {
            logger.Error(actionExecutedContext.Exception.Message);
            var response = new CustomResponse<string>(actionExecutedContext.Request, (int)ResultStatus.Failed, actionExecutedContext.Exception.Message);
            actionExecutedContext.Response = actionExecutedContext.Request.CreateResponse(HttpStatusCode.BadRequest, response.dataWrapper);
        }
    }
}