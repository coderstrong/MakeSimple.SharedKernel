using MakeSimple.SharedKernel.Contract;
using System.Net;
using Xunit;

namespace MakeSimple.SharedKernel.Infrastructure.Test.Exceptions
{
    public class ValidationExceptionTest
    {
        [Fact]
        public void CreateResponseException_Success()
        {
            var result = Error.Create("InternalServerError", "Internal Server Error", HttpStatusCode.InternalServerError);

            Assert.NotNull(result.Code);
            Assert.NotNull(result.Message);
            Assert.NotNull(result.TraceId);
        }
    }
}