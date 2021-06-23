using AutoMapper;
using MakeSimple.SharedKernel.Contract;
using MakeSimple.SharedKernel.Infrastructure.DTO;
using MakeSimple.SharedKernel.Infrastructure.Test.Mocks;
using MakeSimple.SharedKernel.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Xunit;

namespace MakeSimple.SharedKernel.Infrastructure.Test.Repository
{
    public class EfAuditUuidRepositoryGenericTest
    {
        private readonly IAuditRepositoryGeneric<MyDBContext, User> _repositoryGeneric;

        public EfAuditUuidRepositoryGenericTest()
        {
            var config = new MapperConfiguration(cfg => cfg.CreateMap<User, UserDto>().ReverseMap());

            _repositoryGeneric = new EfAuditUuidRepositoryGeneric<MyDBContext, User>(
                new MyDBContext(), new Mapper(config), DummyDataForTest.CreateHttpContext());
        }

        [Fact]
        public async Task Delete_SuccessAsync()
        {
            User u = new User();
            u.Id = Guid.NewGuid();

            _repositoryGeneric.Insert(u);
            await _repositoryGeneric.UnitOfWork.SaveEntitiesAsync();
            _repositoryGeneric.Delete(u.Id);

            var result = await _repositoryGeneric.UnitOfWork.SaveEntitiesAsync();
            Assert.NotNull(u.CreatedBy);
            Assert.True(result);
        }

        [Fact]
        public async Task Update_SuccessAsync()
        {
            User u = new User();
            u.Id = Guid.NewGuid();

            _repositoryGeneric.Insert(u);
            await _repositoryGeneric.UnitOfWork.SaveEntitiesAsync();
            u.UserName = "Test";
            _repositoryGeneric.Update(u);

            var result = await _repositoryGeneric.UnitOfWork.SaveEntitiesAsync();
            var dataFromDb = await _repositoryGeneric.FirstOrDefaultAsync(u.Id);
            Assert.True(result);
            Assert.NotNull(u.ModifiedAt);
            Assert.NotNull(u.ModifiedBy);
            Assert.Equal(dataFromDb.UserName, u.UserName);
        }

        [Fact]
        public async Task GetAllAsync_Success()
        {
            int start = 10, end = 15;
            List<Guid> saveIds = new List<Guid>();
            List<User> Users = new List<User>();
            for (int i = start; i < end; i++)
            {
                User u = new User();
                u.Id = Guid.NewGuid();
                saveIds.Add(u.Id);
                Users.Add(u);
            }
            await _repositoryGeneric.InsertRangeAsync(Users);
            await _repositoryGeneric.UnitOfWork.SaveEntitiesAsync();

            var filters = new List<Expression<Func<User, bool>>> { e => saveIds.Contains(e.Id) };

            var result = await _repositoryGeneric.ToList(filters);

            Assert.True(result.Count == saveIds.Count);
            var idResults = result.Select(e => e.Id).OrderBy(e => e).ToList();
            saveIds = saveIds.OrderBy(e => e).ToList();
            foreach (var item in result)
            {
                Assert.NotNull(item.CreatedBy);
            }
            for (int i = 0; i < idResults.Count; i++)
            {
                Assert.True(idResults[i] == saveIds[i]);
            }
        }

        [Fact]
        public async Task GetAllAsync_Withmapper_Success()
        {
            int start = 16, end = 20;
            List<Guid> saveIds = new List<Guid>();
            List<User> Users = new List<User>();
            for (int i = start; i < end; i++)
            {
                User u = new User();
                u.Id = Guid.NewGuid();
                saveIds.Add(u.Id);
                Users.Add(u);
            }
            await _repositoryGeneric.InsertRangeAsync(Users);
            await _repositoryGeneric.UnitOfWork.SaveEntitiesAsync();

            var filters = new List<Expression<Func<User, bool>>> { e => saveIds.Contains(e.Id) };

            var result = await _repositoryGeneric.ToList<UserDto>(filters);

            Assert.True(result.Count == saveIds.Count);
            var idResults = result.Select(e => e.Id).OrderBy(e => e).ToList();
            saveIds = saveIds.OrderBy(e => e).ToList();
            foreach (var item in Users)
            {
                Assert.NotNull(item.CreatedBy);
            }
            for (int i = 0; i < idResults.Count; i++)
            {
                Assert.True(idResults[i] == saveIds[i]);
            }
        }

        [Fact]
        public async Task GetAllAsync_NotFound_Success()
        {
            int start = 21, end = 25;
            List<Guid> saveIds = new List<Guid>();
            List<User> Users = new List<User>();
            for (int i = start; i < end; i++)
            {
                User u = new User();
                u.Id = Guid.NewGuid();
                saveIds.Add(u.Id);
                Users.Add(u);
            }
            await _repositoryGeneric.InsertRangeAsync(Users);
            await _repositoryGeneric.UnitOfWork.SaveEntitiesAsync();

            var filters = new List<Expression<Func<User, bool>>> { e => e.Id == Guid.NewGuid() };

            var result = await _repositoryGeneric.ToList(filters);
            Assert.True(result.Count == 0);
        }

        [Fact]
        public async Task GetAllAsync_WithMapper_NotFound_Success()
        {
            int start = 26, end = 30;
            List<Guid> saveIds = new List<Guid>();
            List<User> Users = new List<User>();
            for (int i = start; i < end; i++)
            {
                User u = new User();
                u.Id = Guid.NewGuid();
                saveIds.Add(u.Id);
                Users.Add(u);
            }
            await _repositoryGeneric.InsertRangeAsync(Users);
            await _repositoryGeneric.UnitOfWork.SaveEntitiesAsync();

            var filters = new List<Expression<Func<User, bool>>> { e => e.Id == Guid.NewGuid() };

            var result = await _repositoryGeneric.ToList<UserDto>(filters);
            Assert.True(result.Count == 0);
        }

        [Fact]
        public async Task GetFirstOrDefaultAsync_NotFound_Success()
        {
            int start = 31, end = 35;
            List<Guid> saveIds = new List<Guid>();
            List<User> Users = new List<User>();
            for (int i = start; i < end; i++)
            {
                User u = new User();
                u.Id = Guid.NewGuid();
                saveIds.Add(u.Id);
                Users.Add(u);
            }
            await _repositoryGeneric.InsertRangeAsync(Users);
            await _repositoryGeneric.UnitOfWork.SaveEntitiesAsync();

            var result = await _repositoryGeneric.FirstOrDefaultAsync(saveIds[3]);

            Assert.NotNull(result);
            Assert.Equal(result.Id, saveIds[3]);
        }

        [Fact]
        public async Task GetFirstOrDefaultAsync_WithMapper_NotFound_Success()
        {
            int start = 36, end = 40;
            List<Guid> saveIds = new List<Guid>();
            List<User> Users = new List<User>();
            for (int i = start; i < end; i++)
            {
                User u = new User();
                u.Id = Guid.NewGuid();
                saveIds.Add(u.Id);
                Users.Add(u);
            }
            await _repositoryGeneric.InsertRangeAsync(Users);
            await _repositoryGeneric.UnitOfWork.SaveEntitiesAsync();

            var result = await _repositoryGeneric.FirstOrDefaultAsync<UserDto>(saveIds[2]);

            Assert.NotNull(result);
            Assert.Equal(result.Id, saveIds[2]);
        }

        [Fact]
        public async Task GetFirstOrDefaultAsync_Full_Parram_Success()
        {
            int start = 146, end = 150;
            Dictionary<Guid, long> saveIds = new Dictionary<Guid, long>();
            List<User> Users = new List<User>();
            for (int i = start; i < end; i++)
            {
                User u = new User();
                var ad = new Address();
                ad.Id = i;
                u.Address = new List<Address>()
                {
                    ad
                };
                u.Id = Guid.NewGuid();
                saveIds.Add(u.Id, i);
                Users.Add(u);
            }
            await _repositoryGeneric.InsertRangeAsync(Users);
            await _repositoryGeneric.UnitOfWork.SaveEntitiesAsync();

            var result = await _repositoryGeneric.FirstOrDefaultAsync(saveIds.First().Key, e => e.Address);

            Assert.NotNull(result);
            Assert.NotNull(result.Address);
            Assert.Equal(result.Id, saveIds.First().Key);
        }

        [Fact]
        public async Task GetAllAsync_Full_Parrams_Success()
        {
            int start = 151, end = 155;
            List<Guid> saveIds = new List<Guid>();
            List<User> Users = new List<User>();
            for (int i = start; i < end; i++)
            {
                User u = new User();
                var ad = new Address();
                ad.Id = i;
                u.Address = new List<Address>()
                {
                    ad
                };
                u.Id = Guid.NewGuid();
                saveIds.Add(u.Id);
                Users.Add(u);
            }
            await _repositoryGeneric.InsertRangeAsync(Users);
            await _repositoryGeneric.UnitOfWork.SaveEntitiesAsync();

            var filters = new List<Expression<Func<User, bool>>> { e => saveIds.Contains(e.Id) };

            var result = await _repositoryGeneric.ToList(filters
                , e => e.OrderBy(x => x.Id).ThenBy(c => c.UserName)
                , new PaginationQuery(), e => e.Address);

            Assert.True(result.Count == saveIds.Count);
            saveIds = saveIds.OrderBy(e => e).ToList();
            for (int i = 0; i < result.Count; i++)
            {
                Assert.True(result[i].Id == saveIds[i]);
                Assert.NotNull(result[i].Address);
            }
        }

        [Fact]
        public async Task GetAllAsync_Paging_without_Orderby_Success()
        {
            int start = 156, end = 160;
            List<Guid> saveIds = new List<Guid>();
            List<User> Users = new List<User>();
            for (int i = start; i < end; i++)
            {
                User u = new User();
                var ad = new Address();
                ad.Id = i;
                u.Address = new List<Address>()
                {
                    ad
                };
                u.Id = Guid.NewGuid();
                saveIds.Add(u.Id);
                Users.Add(u);
            }
            await _repositoryGeneric.InsertRangeAsync(Users);
            await _repositoryGeneric.UnitOfWork.SaveEntitiesAsync();

            var filters = new List<Expression<Func<User, bool>>> { e => saveIds.Contains(e.Id) };

            var result = await _repositoryGeneric.ToList(filters
                , null
                , new PaginationQuery(), e => e.Address);

            Assert.True(result.Count == saveIds.Count);
            saveIds = saveIds.OrderByDescending(e => e).ToList();
            for (int i = 0; i < result.Count; i++)
            {
                Assert.True(result[i].Id == saveIds[i]);
                Assert.NotNull(result[i].Address);
            }
        }
    }
}