using SchoolWebAPI.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace SchoolWebAPI.Controllers
{
    public class TestController : ApiController
    {
        [HttpGet]
        [CustomExceptionFilter]
        public IHttpActionResult Get()
        {
            int deno = 30 * 2 - 60;
            int data = 100 / deno;
            return Ok();
        }
    }
}
