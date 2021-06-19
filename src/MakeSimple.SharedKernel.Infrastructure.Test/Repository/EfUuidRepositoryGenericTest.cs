using AutoMapper;
using MakeSimple.SharedKernel.Infrastructure.Test.Mocks;
using MakeSimple.SharedKernel.Repository;
using System;
using System.Threading.Tasks;
using Xunit;

namespace MakeSimple.SharedKernel.Infrastructure.Test.Repository
{
    public class EfUuidRepositoryGenericTest
    {
        private readonly EfUuidRepositoryGeneric<MyDBContext, Class> _repositoryGeneric;

        public EfUuidRepositoryGenericTest()
        {
            var config = new MapperConfiguration(cfg => cfg.CreateMap<Class, Class>());

            _repositoryGeneric = new EfUuidRepositoryGeneric<MyDBContext, Class>(
                new MyDBContext(), new Mapper(config));
        }

        [Fact]
        public async Task Delete_SuccessAsync()
        {
            Class u = new Class();
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
            Class u = new Class();
            u.Id = Guid.NewGuid().ToString();

            _repositoryGeneric.Insert(u);
            await _repositoryGeneric.UnitOfWork.SaveEntitiesAsync();
            u.Name = "Test";
            _repositoryGeneric.Update(u);

            var result = await _repositoryGeneric.UnitOfWork.SaveEntitiesAsync();

            Assert.True(result);
            Assert.NotNull(u.ModifiedAt);
        }
    }
}