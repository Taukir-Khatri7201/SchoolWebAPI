using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using SchoolWebAPI.Controllers;
using SchoolWebAPI.Security;
using SchoolWebAPI.Utility;

namespace SchoolWebAPI.Filters
{
    /// <summary>
    /// [1] Custom authorization attribute by using Custom Headers
    /// </summary>
    //public class CustomAuthorizationFilterAttribute : AuthorizationFilterAttribute
    //{
    //    public override void OnAuthorization(HttpActionContext actionContext)
    //    {
    //        if (actionContext.Request.Headers.Contains("username") == false || actionContext.Request.Headers.Contains("password") == false)
    //        {
    //            var response = new CustomResponse<string>(actionContext.Request, (int)ResultStatus.Failed, "Please provide credentials!");
    //            actionContext.Response = actionContext.Request.CreateResponse(HttpStatusCode.Unauthorized, response.dataWrapper);
    //        }
    //        else
    //        {
    //            string username = actionContext.Request.Headers.GetValues("username").First();
    //            string password = actionContext.Request.Headers.GetValues("password").First();
    //            if (!LoginSecurity.Login(username, password))
    //            {
    //                var response = new CustomResponse<string>(actionContext.Request, (int)ResultStatus.Failed, "Invalid username or password!");
    //                actionContext.Response = actionContext.Request.CreateResponse(HttpStatusCode.Unauthorized, response.dataWrapper);
    //            }
    //        }
    //    }
    //}

    /// <summary>
    /// [2] Custom authorization attribute by using Authorization Header
    /// </summary>
    public class CustomAuthorizationFilterAttribute : AuthorizationFilterAttribute
    {
        public override void OnAuthorization(HttpActionContext actionContext)
        {
            if (actionContext.Request.Headers.Authorization == null)
            {
                var response = new CustomResponse<string>(actionContext.Request, (int)ResultStatus.Failed, "Please provide credentials!");
                actionContext.Response = actionContext.Request.CreateResponse(HttpStatusCode.Unauthorized, response.dataWrapper);
            }
            else
            {
                string[] splited = actionContext.Request.Headers.Authorization.ToString().Split('$');
                if (splited.Length != 2 || !LoginSecurity.Login(splited[0], splited[1]))
                {
                    var response = new CustomResponse<string>(actionContext.Request, (int)ResultStatus.Failed, "Invalid username or password!");
                    actionContext.Response = actionContext.Request.CreateResponse(HttpStatusCode.Unauthorized, response.dataWrapper);
                }
            }
        }
    }
}