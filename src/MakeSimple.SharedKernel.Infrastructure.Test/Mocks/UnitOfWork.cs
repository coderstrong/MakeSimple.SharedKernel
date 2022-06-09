using AutoMapper;
using MakeSimple.SharedKernel.Contract;
using MakeSimple.SharedKernel.Infrastructure.Repository;
using System.Threading;
using System.Threading.Tasks;

namespace MakeSimple.SharedKernel.Infrastructure.Test.Mocks
{
    public class UnitOfWork : Disposable, IDatabaseContext
    {
        private MyDBContext databaseContext = new MyDBContext();

        public readonly IRepository<MyDBContext, Student> RepositoryGeneric;
        public readonly IRepository<MyDBContext, User> RepositoryAuditGeneric;
        public readonly IRepository<MyDBContext, Address> AddressGeneric;
        public string Uuid => throw new System.NotImplementedException();

        public UnitOfWork()
        {
            var config = new MapperConfiguration(cfg => cfg.CreateMap<Student, StudentDto>().ReverseMap());
            var config2 = new MapperConfiguration(cfg => cfg.CreateMap<User, UserDto>().ReverseMap());
            RepositoryGeneric = new EfRepositoryGeneric<MyDBContext, Student>(
                databaseContext, SieveMock.Create(), new Mapper(config));
            RepositoryAuditGeneric = new EfRepositoryGeneric<MyDBContext, User>(
                databaseContext, SieveMock.Create(), new Mapper(config2));

            var config3 = new MapperConfiguration(cfg => cfg.CreateMap<Address, AddressDto>().ReverseMap());
            AddressGeneric = new EfRepositoryGeneric<MyDBContext, Address>(
                new MyDBContext(), SieveMock.Create(), new Mapper(config3));
        }

        public async Task<bool> SaveAsync(CancellationToken cancellationToken = default)
        {
            return await databaseContext.SaveAsync(cancellationToken);
        }
    }
}