using AutoMapper;
using MakeSimple.SharedKernel.DTO;
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
    public class EfAuditRepositoryGenericTest
    {
        private readonly EfAuditRepositoryGeneric<MyDBContext, Address> _repositoryGeneric;

        public EfAuditRepositoryGenericTest()
        {
            var config = new MapperConfiguration(cfg => cfg.CreateMap<Address, Address>());

            _repositoryGeneric = new EfAuditRepositoryGeneric<MyDBContext, Address>(
                new MyDBContext(), new Mapper(config), DummyDataForTest.CreateHttpContext());
        }

        [Fact]
        public async Task Delete_SuccessAsync()
        {
            Address u = new Address();
            u.Id = 1;

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
            u.Id = 2;

            _repositoryGeneric.Insert(u);
            await _repositoryGeneric.UnitOfWork.SaveEntitiesAsync();
            u.Line1 = "Test";
            _repositoryGeneric.Update(u);

            var result = await _repositoryGeneric.UnitOfWork.SaveEntitiesAsync();
            var dataFromDb = await _repositoryGeneric.FirstOrDefaultAsync(u.Id);
            Assert.True(result);
            Assert.NotNull(u.ModifiedAt);
            Assert.Equal(dataFromDb.Line1, u.Line1);
        }

        [Fact]
        public async Task GetAllAsync_Success()
        {
            int start = 10, end = 15;
            List<long> saveIds = new List<long>();
            List<Address> addresses = new List<Address>();
            for (int i = start; i < end; i++)
            {
                Address u = new Address();
                u.Id = i;
                saveIds.Add(u.Id);
                addresses.Add(u);
            }
            _repositoryGeneric.InsertRange(addresses);
            await _repositoryGeneric.UnitOfWork.SaveEntitiesAsync();

            var filters = new List<Expression<Func<Address, bool>>> { e => saveIds.Contains(e.Id) };

            var result = await _repositoryGeneric.ToList(filters);

            Assert.True(result.Count == saveIds.Count);
            var idResults = result.Select(e => e.Id).OrderBy(e => e).ToList();
            saveIds = saveIds.OrderBy(e => e).ToList();
            for (int i = 0; i < idResults.Count; i++)
            {
                Assert.True(idResults[i] == saveIds[i]);
            }
        }


        [Fact]
        public async Task GetAllAsync_Withmapper_Success()
        {
            int start = 16, end = 20;
            List<long> saveIds = new List<long>();
            List<Address> addresses = new List<Address>();
            for (int i = start; i < end; i++)
            {
                Address u = new Address();
                u.Id = i;
                saveIds.Add(u.Id);
                addresses.Add(u);
            }
            _repositoryGeneric.InsertRange(addresses);
            await _repositoryGeneric.UnitOfWork.SaveEntitiesAsync();

            var filters = new List<Expression<Func<Address, bool>>> { e => saveIds.Contains(e.Id) };

            var result = await _repositoryGeneric.ToList<Address>(filters);

            Assert.True(result.Count == saveIds.Count);
            var idResults = result.Select(e => e.Id).OrderBy(e => e).ToList();
            saveIds = saveIds.OrderBy(e => e).ToList();
            for (int i = 0; i < idResults.Count; i++)
            {
                Assert.True(idResults[i] == saveIds[i]);
            }
        }

        [Fact]
        public async Task GetAllAsync_NotFound_Success()
        {
            int start = 21, end = 25;
            List<long> saveIds = new List<long>();
            List<Address> addresses = new List<Address>();
            for (int i = start; i < end; i++)
            {
                Address u = new Address();
                u.Id = i;
                saveIds.Add(u.Id);
                addresses.Add(u);
            }
            _repositoryGeneric.InsertRange(addresses);
            await _repositoryGeneric.UnitOfWork.SaveEntitiesAsync();

            var filters = new List<Expression<Func<Address, bool>>> { e => e.Id == new Random().Next(5000, 6000) };

            var result = await _repositoryGeneric.ToList(filters);

            Assert.True(result.Count == 0);
        }

        [Fact]
        public async Task GetAllAsync_WithMapper_NotFound_Success()
        {
            int start = 26, end = 30;
            List<long> saveIds = new List<long>();
            List<Address> addresses = new List<Address>();
            for (int i = start; i < end; i++)
            {
                Address u = new Address();
                u.Id = i;
                saveIds.Add(u.Id);
                addresses.Add(u);
            }
            _repositoryGeneric.InsertRange(addresses);
            await _repositoryGeneric.UnitOfWork.SaveEntitiesAsync();

            var filters = new List<Expression<Func<Address, bool>>> { e => e.Id == new Random().Next(5000, 6000) };

            var result = await _repositoryGeneric.ToList<Address>(filters);

            Assert.True(result.Count == 0);
        }

        [Fact]
        public async Task GetFirstOrDefaultAsync_NotFound_Success()
        {
            int start = 31, end = 35;
            List<long> saveIds = new List<long>();
            List<Address> addresses = new List<Address>();
            for (int i = start; i < end; i++)
            {
                Address u = new Address();
                u.Id = i;
                saveIds.Add(u.Id);
                addresses.Add(u);
            }
            _repositoryGeneric.InsertRange(addresses);
            await _repositoryGeneric.UnitOfWork.SaveEntitiesAsync();

            var result = await _repositoryGeneric.FirstOrDefaultAsync(saveIds[3]);

            Assert.NotNull(result);
            Assert.Equal(result.Id, saveIds[3]);
        }

        [Fact]
        public async Task GetFirstOrDefaultAsync_WithMapper_NotFound_Success()
        {
            int start = 36, end = 40;
            List<long> saveIds = new List<long>();
            List<Address> addresses = new List<Address>();
            for (int i = start; i < end; i++)
            {
                Address u = new Address();
                u.Id = i;
                saveIds.Add(u.Id);
                addresses.Add(u);
            }
            _repositoryGeneric.InsertRange(addresses);
            await _repositoryGeneric.UnitOfWork.SaveEntitiesAsync();

            var result = await _repositoryGeneric.FirstOrDefaultAsync<Address>(saveIds[2]);

            Assert.NotNull(result);
            Assert.Equal(result.Id, saveIds[2]);
        }

        [Fact]
        public async Task GetFirstOrDefaultAsync_Full_Parram_Success()
        {
            int start = 46, end = 50;
            Dictionary<long, string> saveIds = new Dictionary<long, string>();
            List<Address> addresses = new List<Address>();
            for (int i = start; i < end; i++)
            {
                Address u = new Address();
                u.User = new User();
                u.User.Id = Guid.NewGuid().ToString();
                u.Id = i;
                saveIds.Add(u.Id, u.User.Id);
                addresses.Add(u);
            }
            _repositoryGeneric.InsertRange(addresses);
            await _repositoryGeneric.UnitOfWork.SaveEntitiesAsync();

            var result = await _repositoryGeneric.FirstOrDefaultAsync(saveIds.First().Key, e => e.User);

            Assert.NotNull(result);
            Assert.NotNull(result.User);
            Assert.Equal(result.Id, saveIds.First().Key);
        }

        [Fact]
        public async Task GetAllAsync_Full_Parrams_Success()
        {
            int start = 51, end = 55;
            List<long> saveIds = new List<long>();
            List<Address> addresses = new List<Address>();
            for (int i = start; i < end; i++)
            {
                Address u = new Address();
                u.User = new User();
                u.User.Id = Guid.NewGuid().ToString();
                u.Id = i;
                saveIds.Add(u.Id);
                addresses.Add(u);
            }
            _repositoryGeneric.InsertRange(addresses);
            await _repositoryGeneric.UnitOfWork.SaveEntitiesAsync();

            var filters = new List<Expression<Func<Address, bool>>> { e => saveIds.Contains(e.Id) };

            var result = await _repositoryGeneric.ToList(filters
                , e => e.OrderBy(x => x.Id).ThenBy(c => c.Line1)
                , new PaginationQuery(), e => e.User);

            Assert.True(result.Count == saveIds.Count);
            saveIds = saveIds.OrderBy(e => e).ToList();
            for (int i = 0; i < result.Count; i++)
            {
                Assert.True(result[i].Id == saveIds[i]);
                Assert.NotNull(result[i].User);
            }
        }

        [Fact]
        public async Task GetAllAsync_Paging_without_Orderby_Success()
        {
            int start = 56, end = 60;
            List<long> saveIds = new List<long>();
            List<Address> addresses = new List<Address>();
            for (int i = start; i < end; i++)
            {
                Address u = new Address();
                u.User = new User();
                u.User.Id = Guid.NewGuid().ToString();
                u.Id = i;
                saveIds.Add(u.Id);
                addresses.Add(u);
            }
            _repositoryGeneric.InsertRange(addresses);
            await _repositoryGeneric.UnitOfWork.SaveEntitiesAsync();

            var filters = new List<Expression<Func<Address, bool>>> { e => saveIds.Contains(e.Id) };

            var result = await _repositoryGeneric.ToList(filters
                , null
                , new PaginationQuery(), e => e.User);

            Assert.True(result.Count == saveIds.Count);
            saveIds = saveIds.OrderByDescending(e => e).ToList();
            for (int i = 0; i < result.Count; i++)
            {
                Assert.True(result[i].Id == saveIds[i]);
                Assert.NotNull(result[i].User);
            }
        }
    }
}