using ArvatoAssessment.DataAccess.Repository;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace ArvatoAssessment.Middleware
{
    public class RequestLoggingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IRequestLogRepository requestLogRepository;

        public RequestLoggingMiddleware(RequestDelegate next)
        {
            _next = next;
            requestLogRepository = new RequestLogRepository();
        }

        public async Task Invoke(HttpContext context)
        {
            //Copy a pointer to the original response body stream
            var originalBodyStream = context.Response.Body;

            //Create a new memory stream
            using (var responseBody = new MemoryStream())
            {
                //temporary response body
                context.Response.Body = responseBody;

                await _next(context);

                context.Response.Body.Seek(0, SeekOrigin.Begin);

                //save the request Log to database
                requestLogRepository.Add(new DataAcess.Entity.RequestLog
                {
                    RequestUrl = context.Request.Host + context.Request.Path,
                    RequestTime = DateTime.UtcNow,
                    ResponseCode = context.Response.StatusCode.ToString(),
                    ResponseDescription = Enum.Parse(typeof(HttpStatusCode), context.Response.StatusCode.ToString()).ToString()
                });

                //Copy the contents to original stream and return to the client.
                await responseBody.CopyToAsync(originalBodyStream);
            }
        }
    }
}
