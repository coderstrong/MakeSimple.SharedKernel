using MakeSimple.SharedKernel.Contract;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace MakeSimple.SharedKernel.Infrastructure.AspNet
{
    public class ControllerApiBase : ControllerBase
    {
        public virtual IActionResult ResultDTO(IDataResult result)
        {
            if (result == null)
            {
                return new NotFoundObjectResult(result);
            }

            return result.StatusCode switch
            {
                HttpStatusCode.OK => Ok(result),
                HttpStatusCode.Conflict => Conflict(result),
                HttpStatusCode.NoContent => NoContent(),
                HttpStatusCode.BadRequest => BadRequest(result),
                HttpStatusCode.NotFound => NotFound(result),
                _ => new CustomObjectResult((int)result.StatusCode, result),
            };
        }

        protected class CustomObjectResult : ObjectResult
        {
            public CustomObjectResult(int statusCode, object value)
                : base(value)
            {
                StatusCode = statusCode;
            }
        }
    }
}
