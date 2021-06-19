using Microsoft.AspNetCore.Http;
using Moq;
using System.Security.Claims;

namespace MakeSimple.SharedKernel.Infrastructure.Test.Mocks
{
    public static class DummyDataForTest
    {
        public static IHttpContextAccessor CreateHttpContext()
        {
            //Mock IHttpContextAccessor
            var mockHttpContextAccessor = new Mock<IHttpContextAccessor>();

            mockHttpContextAccessor.Setup(req => req.HttpContext.User).Returns(It.IsAny<ClaimsPrincipal>());

            return mockHttpContextAccessor.Object;
        }
    }
}