using AutoMapper;
using MakeSimple.SharedKernel.Infrastructure.Test.Mocks;
using MakeSimple.SharedKernel.Repository;
using Microsoft.AspNetCore.Http;
using Moq;
using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Xunit;

namespace MakeSimple.SharedKernel.Infrastructure.Test.Repository
{
    public class EfAuditRepositoryGenericTest
    {
        private readonly EfAuditRepositoryGeneric<MyDBContext, Address> _repositoryGeneric;

        public EfAuditRepositoryGenericTest()
        {
            var config = new MapperConfiguration(cfg => cfg.CreateMap<Address, Address>());

            //Mock IHttpContextAccessor
            var mockHttpContextAccessor = new Mock<IHttpContextAccessor>();

            mockHttpContextAccessor.Setup(req => req.HttpContext.User).Returns(It.IsAny<ClaimsPrincipal>());

            _repositoryGeneric = new EfAuditRepositoryGeneric<MyDBContext, Address>(
                new MyDBContext(), new Mapper(config), mockHttpContextAccessor.Object);

        }

        [Fact]
        public async Task Delete_SuccessAsync()
        {
            Address u = new Address();
            u.Id = new Random().Next();

            _repositoryGeneric.Insert(u);
            await _repositoryGeneric.UnitOfWork.SaveEntitiesAsync();
            _repositoryGeneric.Delete(u.Id);

            var result = await _repositoryGeneric.UnitOfWork.SaveEntitiesAsync();

            Assert.True(result);
        }

        [Fact]
        public async Task Update_SuccessAsync()
        {
            Address u = new Address();
            u.Id = new Random().Next();

            _repositoryGeneric.Insert(u);
            await _repositoryGeneric.UnitOfWork.SaveEntitiesAsync();
            u.Line1 = "Test";
            _repositoryGeneric.Update(u);

            var result = await _repositoryGeneric.UnitOfWork.SaveEntitiesAsync();

            Assert.True(result);
            Assert.NotNull(u.ModifiedAt);
        }
    }
}
