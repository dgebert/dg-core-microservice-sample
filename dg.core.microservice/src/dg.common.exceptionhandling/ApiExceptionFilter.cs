using System;
using System.Net;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;

namespace dg.common.exceptionhandling
{
    /*  Registration: 
       
            services.AddMvc(
                config =>
                {
                    config.Filters.Add(typeof(ErrorHandling.ApiExceptionFilter));
                }
            );
     */

    public class ApiExceptionFilter : IExceptionFilter
    {
        ILogger _logger;
        public ApiExceptionFilter(ILogger<ApiExceptionFilter> logger)
        {
            _logger = logger;
        }

        public void OnException(ExceptionContext context)
        {
            HttpStatusCode status = HttpStatusCode.InternalServerError;
            String message = String.Empty;

            var exception = context.Exception;
            var exceptionType = exception.GetType();
            if (exceptionType == typeof(UnauthorizedAccessException))
            {
                message = "Unauthorized Access";
                status = HttpStatusCode.Unauthorized;
            }
            else if (exceptionType == typeof(NotImplementedException))
            {
                message = "A server error occurred.";
                status = HttpStatusCode.NotImplemented;
            }
            else if (exceptionType == typeof(ApiException))
            {
                message = exception.ToString();
                status = HttpStatusCode.InternalServerError;
            }
            else
            {
                message = exception.Message;
                status = HttpStatusCode.InternalServerError;
            }

            var err = string.Format("{0} {1} {2}", exception.GetType().FullName, message, exception.StackTrace);
            _logger.LogCritical(new EventId(), exception, err);

            HttpResponse response = context.HttpContext.Response;
            response.StatusCode = (int)status;
            response.ContentType = "application/json";

            var jsonError = Newtonsoft.Json.JsonConvert.SerializeObject(
                new
                {
                    exception = exception.GetType().FullName,
                    message = exception.Message,
                    stacktrace = exception.StackTrace
                });
            response.WriteAsync(jsonError);
        }
    }
    
}
