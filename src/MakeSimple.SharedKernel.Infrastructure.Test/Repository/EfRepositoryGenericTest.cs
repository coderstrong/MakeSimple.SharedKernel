using AutoMapper;
using MakeSimple.SharedKernel.Infrastructure.Test.Mocks;
using MakeSimple.SharedKernel.Repository;
using System;
using System.Threading.Tasks;
using Xunit;

namespace MakeSimple.SharedKernel.Infrastructure.Test.Repository
{
    public class EfRepositoryGenericTest
    {
        private readonly EfRepositoryGeneric<MyDBContext, Student> _repositoryGeneric;

        public EfRepositoryGenericTest()
        {
            var config = new MapperConfiguration(cfg => cfg.CreateMap<Student, Student>());

            _repositoryGeneric = new EfRepositoryGeneric<MyDBContext, Student>(
                new MyDBContext(), new Mapper(config));
        }

        [Fact]
        public async Task Delete_SuccessAsync()
        {
            Student u = new Student();
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
            Student u = new Student();
            u.Id = new Random().Next();

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