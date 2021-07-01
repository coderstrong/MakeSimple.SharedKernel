using AutoMapper;
using MakeSimple.SharedKernel.Contract;
using MakeSimple.SharedKernel.Infrastructure.Test.Mocks;
using MakeSimple.SharedKernel.Repository;
using System.Threading.Tasks;
using Xunit;

namespace MakeSimple.SharedKernel.Infrastructure.Test.Entity
{
    public class EntityTest
    {
        private readonly IAuditRepositoryGeneric<MyDBContext, Address> _repositoryGeneric;

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