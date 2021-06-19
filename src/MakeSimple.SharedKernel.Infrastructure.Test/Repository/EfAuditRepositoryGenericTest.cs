using AutoMapper;
using MakeSimple.SharedKernel.Infrastructure.Test.Mocks;
using MakeSimple.SharedKernel.Repository;
using Microsoft.AspNetCore.Http;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
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

        [Fact]
        public async Task GetAllAsync_Success()
        {
            List<long> saveIds = new List<long>();

            for (int i = 0; i < 10; i++)
            {
                Address u = new Address();
                u.Id = new Random().Next();
                saveIds.Add(u.Id);
                _repositoryGeneric.Insert(u);
            }

            await _repositoryGeneric.UnitOfWork.SaveEntitiesAsync();

            var filters = new List<Expression<Func<Address, bool>>> { e => saveIds.Contains(e.Id) };

            var result = await _repositoryGeneric.GetAllAsync(filters);

            Assert.True(result.Count == saveIds.Count);
            var idResults = result.Select(e => e.Id).OrderBy(e => e).ToList();
            saveIds = saveIds.OrderBy(e => e).ToList();
            for (int i = 0; i < idResults.Count; i++)
            {
                Assert.True(idResults[i] == saveIds[i]);
            }
        }
    }
}