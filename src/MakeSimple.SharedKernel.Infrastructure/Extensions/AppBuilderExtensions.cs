using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Serilog;
using System;
using System.Net;
using System.Text.Json;

namespace MakeSimple.SharedKernel.Infrastructure.Extensions
{
    using MakeSimple.SharedKernel.Contract;
    using MakeSimple.SharedKernel.Infrastructure.DTO;
    using MakeSimple.SharedKernel.Infrastructure.Exceptions;

    public static class AppBuilderExtensions
    {
        public static IApplicationBuilder UseExceptionHandlerCore(this IApplicationBuilder app)
        {
            app.Use(async (context, next) =>
            {
                context.Request.EnableBuffering();
                await next.Invoke();
            });

            app.UseExceptionHandler(errorApp =>
            {
                errorApp.Run(async context =>
                {
                    IDataResult result;

                    var exceptionHandlerPathFeature = context.Features.Get<IExceptionHandlerPathFeature>();
                    var exception = exceptionHandlerPathFeature?.Error;
                    if (exception != null)
                    {
                        if (exception is BaseException baseException)
                        {
                            result = baseException.DataResult;
                        }
                        else
                        {
                            result = new Response<bool>
                            (
                                HttpStatusCode.InternalServerError,
                                new ErrorBase()
                                {
                                    Code = "InternalServerError",
                                    ErrorMessage = "Internal Server Error"
                                }
                            );
                        }
                    }
                    else
                    {
                        exception = new Exception("Unhandled");

                        result = new Response<bool>
                        (
                            HttpStatusCode.InternalServerError,
                            new ErrorBase()
                            {
                                Code = "Unhandled",
                                ErrorMessage = "Unhandled"
                            }
                        );
                    }

                    string jsonResult = JsonSerializer.Serialize(result);
                    string requestInfo = $"HTTP Request:\n" +
                    $"\tClaims: {context.User?.CovertToString()}\n" +
                    $"\tHeaders: {context.Request.Headers?.CovertToString()}\n" +
                    $"\tBody: {await context.Request.Body?.ReadToEndBufferingAsync()}\n" +
                    $"\tQueryString: {context.Request.QueryString}\n" +
                    $"\tResponse: {jsonResult}\n";

                    Log.Error(exception.GetBaseException(), requestInfo);

                    context.Response.StatusCode = (int)result.StatusCode;
                    context.Response.ContentType = "application/json";
                    await context.Response.WriteAsync(jsonResult);
                });
            });

            return app;
        }
    }
}