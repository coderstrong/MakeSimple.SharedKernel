using MakeSimple.SharedKernel.Contract;
using MakeSimple.SharedKernel.Infrastructure.DTO;
using MakeSimple.SharedKernel.Infrastructure.Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;

namespace MakeSimple.SharedKernel.Infrastructure.AspNet
{

    public class HandleExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<HandleExceptionMiddleware> _logger;

        public HandleExceptionMiddleware(RequestDelegate next, ILogger<HandleExceptionMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task Invoke(HttpContext context)
        {
            if (IsDefault(context))
            {
                await _next(context);
            }
            else
            {
                try
                {
                    await _next.Invoke(context);
                }
                catch (Exception ex)
                {
                    await HandleExceptionAsync(context, ex);
                }
            }
        }

        private Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            IDataResult result;

            if (exception is BaseException baseException)
            {
                result = baseException.DataResult;
            }
            else
            {
                result = new SingleResult<bool>
                {
                    Error = new ErrorBase()
                    {
                        Code = "Unhandled",
                        ErrorMessage = exception.Message
                    },
                    StatusCode = HttpStatusCode.InternalServerError,
                    Item = false
                };
            }

            _logger.LogError(exception, "{@result}", result);

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)result.StatusCode;
            return context.Response.WriteAsync(JsonSerializer.Serialize(result));
        }

        private bool IsDefault(HttpContext context)
        {
            return context.Request.Path.StartsWithSegments("/");
        }
    }
}
