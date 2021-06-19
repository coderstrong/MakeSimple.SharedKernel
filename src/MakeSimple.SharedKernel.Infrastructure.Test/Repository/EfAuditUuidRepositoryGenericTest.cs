using AutoMapper;
using MakeSimple.SharedKernel.Infrastructure.Test.Mocks;
using MakeSimple.SharedKernel.Repository;
using System;
using System.Threading.Tasks;
using Xunit;

namespace MakeSimple.SharedKernel.Infrastructure.Test.Repository
{
    public class EfAuditUuidRepositoryGenericTest
    {
        private readonly EfAuditUuidRepositoryGeneric<MyDBContext, User> _repositoryGeneric;

        public EfAuditUuidRepositoryGenericTest()
        {
            var config = new MapperConfiguration(cfg => cfg.CreateMap<User, User>());

            _repositoryGeneric = new EfAuditUuidRepositoryGeneric<MyDBContext, User>(
                new MyDBContext(), new Mapper(config), DummyDataForTest.CreateHttpContext());
        }

        [Fact]
        public async Task Delete_SuccessAsync()
        {
            User u = new User();
            u.Id = Guid.NewGuid().ToString();

            _repositoryGeneric.Insert(u);
            await _repositoryGeneric.UnitOfWork.SaveEntitiesAsync();
            _repositoryGeneric.Delete(u.Id);

            var result = await _repositoryGeneric.UnitOfWork.SaveEntitiesAsync();

            Assert.True(result);
        }

        [Fact]
        public async Task Update_SuccessAsync()
        {
            User u = new User();
            u.Id = Guid.NewGuid().ToString();

            _repositoryGeneric.Insert(u);
            await _repositoryGeneric.UnitOfWork.SaveEntitiesAsync();
            u.Password = "Test";
            _repositoryGeneric.Update(u);

            var result = await _repositoryGeneric.UnitOfWork.SaveEntitiesAsync();

            Assert.True(result);
            Assert.NotNull(u.ModifiedAt);
        }
    }
}