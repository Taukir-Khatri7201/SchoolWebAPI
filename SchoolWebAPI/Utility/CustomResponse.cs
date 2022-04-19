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
    public class CustomDataWrapper<T>
    {
        public T Data { get; set; }
        public int StatusCode { get; set; }
        public List<string> messages { get; set; }
    }

    public class CustomResponse<T> : IHttpActionResult
    {
        public CustomDataWrapper<T> dataWrapper { get; set; }
        private readonly HttpRequestMessage request;

        public CustomResponse(HttpRequestMessage requestMessage, int code, string msg, T _data = default)
        {
            dataWrapper = new CustomDataWrapper<T>()
            {
                Data = _data,
                StatusCode = code,
            };
            dataWrapper.messages = new List<string>();
            if(!string.IsNullOrEmpty(msg)) dataWrapper.messages.Add(msg);

            request = requestMessage;
        }

        public CustomResponse(HttpRequestMessage requestMessage, int code, List<string> msgs, T _data = default)
        {
            dataWrapper = new CustomDataWrapper<T>()
            {
                Data = _data,
                StatusCode = code,
            };
            dataWrapper.messages = new List<string>();
            dataWrapper.messages.AddRange(msgs);
            request = requestMessage;
        }

        public Task<HttpResponseMessage> ExecuteAsync(CancellationToken cancellationToken)
        {
            HttpResponseMessage response = request.CreateResponse(HttpStatusCode.OK, dataWrapper);
            return Task.FromResult(response);
        }
    }
}