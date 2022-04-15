using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;

namespace SchoolWebAPI.Utility
{
    public class CustomResponse<T1, T2> : IHttpActionResult
    {
        public T1 data { get; private set; }
        private readonly int statusCode;
        private readonly string messages;
        private readonly HttpRequestMessage request;

        public CustomResponse(HttpRequestMessage requestMessage, int code, T2 msg, T1 _data = default)
        {
            data = _data;
            statusCode = code;
            switch(msg.GetType().ToString())
            {
                case "System.String":
                    messages = msg as string;
                    break;
                case "System.Collections.Generic.List`1[System.String]":
                    messages = String.Join(", ", msg as List<string>);
                    break;
            }
            //messages = string.Join(",", msg).ToString();
            request = requestMessage;
        }

        public Task<HttpResponseMessage> ExecuteAsync(CancellationToken cancellationToken)
        {
            HttpResponseMessage response;
            if(messages.Length > 0)
            {
                response = request.CreateResponse((HttpStatusCode)statusCode, messages);
            }
            else
            {
                response = request.CreateResponse((HttpStatusCode)statusCode, data);
            }
            return Task.FromResult(response);
        }
    }
}