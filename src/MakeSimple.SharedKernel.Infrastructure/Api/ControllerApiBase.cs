using MakeSimple.SharedKernel.Contract;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace MakeSimple.SharedKernel.Infrastructure.Api
{
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    [Authorize]
    public class ControllerApiBase : ControllerBase
    {
        protected virtual IActionResult ResultDTO(IDataResult result)
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
                _ => new ObjectResult(result)
                {
                    StatusCode = (int)result.StatusCode
                }
            };
        }
    }
}