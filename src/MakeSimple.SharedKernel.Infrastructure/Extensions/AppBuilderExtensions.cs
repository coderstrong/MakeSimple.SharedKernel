using MakeSimple.SharedKernel.Infrastructure.AspNet;
using Microsoft.AspNetCore.Builder;

namespace MakeSimple.SharedKernel.Infrastructure.Extensions
{
    public static class AppBuilderExtensions
    {
        public static IApplicationBuilder UseExceptionHandlerCore(this IApplicationBuilder app)
        {
            return app.UseMiddleware<HandleExceptionMiddleware>();
        }
    }

}
