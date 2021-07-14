using MakeSimple.SharedKernel.Infrastructure.DTO;
using System.Net;
using Xunit;

namespace MakeSimple.SharedKernel.Infrastructure.Test.Exceptions
{
    public class ValidationExceptionTest
    {
        [Fact]
        public void CreateResponseException_Success()
        {
            var result = new Response<bool>
                            (
                                HttpStatusCode.InternalServerError,
                                new ErrorBase("InternalServerError", "Internal Server Error")
                            );

            Assert.NotNull(result.Error);
            Assert.NotNull(result.Error.TraceId);
        }
    }
}