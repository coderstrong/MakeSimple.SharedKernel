using MakeSimple.SharedKernel.Infrastructure.Test.Mocks;
using System.Threading.Tasks;
using Xunit;

namespace MakeSimple.SharedKernel.Infrastructure.Test.Entities
{
    public class EntityTest
    {
        private readonly UnitOfWork _unit;
        public EntityTest()
        {
            _unit = new UnitOfWork();
        }

        //[Fact]
        public async Task CompareEntity_SuccessAsync()
        {
            Address u = new Address();
            u.Id = 1000;

            _unit.AddressGeneric.Insert(u);
            await _unit.SaveAsync();

            var addressFromDb = await _unit.AddressGeneric.FirstOrDefaultAsync(u.Id);

            var aa = addressFromDb.Equals(u);
            Assert.True(aa);
        }
    }
}