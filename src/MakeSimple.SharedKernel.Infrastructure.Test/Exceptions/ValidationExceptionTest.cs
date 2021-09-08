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
            var code = "InternalServerError";
            var mess = "Internal Server Error";
            var result = Error.Create(code, mess);

            Assert.NotNull(result.Code);
            Assert.Equal(result.Code, code);
            Assert.NotNull(result.Message);
            Assert.Equal(result.Message, mess);
            Assert.NotNull(result.TraceId);
        }
    }
}