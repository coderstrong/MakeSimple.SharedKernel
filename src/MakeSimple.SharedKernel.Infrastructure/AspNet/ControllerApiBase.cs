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

            switch (result.StatusCode)
            {
                case HttpStatusCode.OK:
                    return Ok(result);

                case HttpStatusCode.NoContent:
                    return NoContent();

                case HttpStatusCode.BadRequest:
                    return BadRequest(result);

                case HttpStatusCode.NotFound:
                    return NotFound(result);

                default:
                    return new CustomObjectResult((int)result.StatusCode, result);
            }
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
