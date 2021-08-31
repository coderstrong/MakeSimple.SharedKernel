﻿using AutoMapper;
using MakeSimple.SharedKernel.Contract;
using MakeSimple.SharedKernel.Infrastructure.Repository;
using MakeSimple.SharedKernel.Infrastructure.Test.Mocks;
using System.Threading.Tasks;
using Xunit;

namespace MakeSimple.SharedKernel.Infrastructure.Test.Entities
{
    public class EntityTest
    {
        private readonly IAuditRepository<MyDBContext, Address> _repositoryGeneric;

        public EntityTest()
        {
            var config = new MapperConfiguration(cfg => cfg.CreateMap<Address, AddressDto>().ReverseMap());
            _repositoryGeneric = new EfAuditRepositoryGeneric<MyDBContext, Address>(
                new MyDBContext(), SieveMock.Create(), new Mapper(config), DummyDataForTest.CreateHttpContext());
        }

        //[Fact]
        public async Task CompareEntity_SuccessAsync()
        {
            Address u = new Address();
            u.Id = 1000;

            _repositoryGeneric.Insert(u);
            await _repositoryGeneric.UnitOfWork.SaveEntitiesAsync();

            var addressFromDb = await _repositoryGeneric.FirstOrDefaultAsync(u.Id);

            var aa = addressFromDb.Equals(u);
            Assert.True(aa);
        }
    }
}