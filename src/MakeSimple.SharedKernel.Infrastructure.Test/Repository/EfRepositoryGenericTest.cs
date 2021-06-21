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
            Student u = new Student();
            u.Id = 2;

            _repositoryGeneric.Insert(u);
            await _repositoryGeneric.UnitOfWork.SaveEntitiesAsync();
            u.Name = "Test";
            _repositoryGeneric.Update(u);

            var result = await _repositoryGeneric.UnitOfWork.SaveEntitiesAsync();
            var dataFromDb = await _repositoryGeneric.FirstOrDefaultAsync(u.Id);
            Assert.True(result);
            Assert.NotNull(u.ModifiedAt);
            Assert.Equal(dataFromDb.Name, u.Name);
        }

        [Fact]
        public async Task GetAllAsync_Success()
        {
            int start = 10, end = 15;
            List<long> saveIds = new List<long>();
            List<Student> Studentes = new List<Student>();
            for (int i = start; i < end; i++)
            {
                Student u = new Student();
                u.Id = i;
                saveIds.Add(u.Id);
                Studentes.Add(u);
            }
            await _repositoryGeneric.InsertRangeAsync(Studentes);
            await _repositoryGeneric.UnitOfWork.SaveEntitiesAsync();

            var filters = new List<Expression<Func<Student, bool>>> { e => saveIds.Contains(e.Id) };

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
            List<Student> Studentes = new List<Student>();
            for (int i = start; i < end; i++)
            {
                Student u = new Student();
                u.Id = i;
                saveIds.Add(u.Id);
                Studentes.Add(u);
            }
            await _repositoryGeneric.InsertRangeAsync(Studentes);
            await _repositoryGeneric.UnitOfWork.SaveEntitiesAsync();

            var filters = new List<Expression<Func<Student, bool>>> { e => saveIds.Contains(e.Id) };

            var result = await _repositoryGeneric.ToList<Student>(filters);

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
            List<Student> Studentes = new List<Student>();
            for (int i = start; i < end; i++)
            {
                Student u = new Student();
                u.Id = i;
                saveIds.Add(u.Id);
                Studentes.Add(u);
            }
            await _repositoryGeneric.InsertRangeAsync(Studentes);
            await _repositoryGeneric.UnitOfWork.SaveEntitiesAsync();

            var filters = new List<Expression<Func<Student, bool>>> { e => e.Id == new Random().Next(5000, 6000) };

            var result = await _repositoryGeneric.ToList(filters);

            Assert.True(result.Count == 0);
        }

        [Fact]
        public async Task GetAllAsync_WithMapper_NotFound_Success()
        {
            int start = 26, end = 30;
            List<long> saveIds = new List<long>();
            List<Student> Studentes = new List<Student>();
            for (int i = start; i < end; i++)
            {
                Student u = new Student();
                u.Id = i;
                saveIds.Add(u.Id);
                Studentes.Add(u);
            }
            await _repositoryGeneric.InsertRangeAsync(Studentes);
            await _repositoryGeneric.UnitOfWork.SaveEntitiesAsync();

            var filters = new List<Expression<Func<Student, bool>>> { e => e.Id == new Random().Next(5000, 6000) };

            var result = await _repositoryGeneric.ToList<Student>(filters);

            Assert.True(result.Count == 0);
        }

        [Fact]
        public async Task GetFirstOrDefaultAsync_NotFound_Success()
        {
            int start = 31, end = 35;
            List<long> saveIds = new List<long>();
            List<Student> Studentes = new List<Student>();
            for (int i = start; i < end; i++)
            {
                Student u = new Student();
                u.Id = i;
                saveIds.Add(u.Id);
                Studentes.Add(u);
            }
            await _repositoryGeneric.InsertRangeAsync(Studentes);
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
            List<Student> Studentes = new List<Student>();
            for (int i = start; i < end; i++)
            {
                Student u = new Student();
                u.Id = i;
                saveIds.Add(u.Id);
                Studentes.Add(u);
            }
            await _repositoryGeneric.InsertRangeAsync(Studentes);
            await _repositoryGeneric.UnitOfWork.SaveEntitiesAsync();

            var result = await _repositoryGeneric.FirstOrDefaultAsync<Student>(saveIds[2]);

            Assert.NotNull(result);
            Assert.Equal(result.Id, saveIds[2]);
        }

        [Fact]
        public async Task GetFirstOrDefaultAsync_Full_Parram_Success()
        {
            int start = 46, end = 50;
            Dictionary<long, Guid> saveIds = new Dictionary<long, Guid>();
            List<Student> Studentes = new List<Student>();
            for (int i = start; i < end; i++)
            {
                Student u = new Student();
                u.Class = new Class();
                u.Class.Id = Guid.NewGuid();
                u.Id = i;
                saveIds.Add(u.Id, u.Class.Id);
                Studentes.Add(u);
            }
            await _repositoryGeneric.InsertRangeAsync(Studentes);
            await _repositoryGeneric.UnitOfWork.SaveEntitiesAsync();

            var result = await _repositoryGeneric.FirstOrDefaultAsync(saveIds.First().Key, e => e.Class);

            Assert.NotNull(result);
            Assert.NotNull(result.Class);
            Assert.Equal(result.Id, saveIds.First().Key);
        }

        [Fact]
        public async Task GetAllAsync_Full_Parrams_Success()
        {
            int start = 51, end = 55;
            List<long> saveIds = new List<long>();
            List<Student> Studentes = new List<Student>();
            for (int i = start; i < end; i++)
            {
                Student u = new Student();
                u.Class = new Class();
                u.Class.Id = Guid.NewGuid();
                u.Id = i;
                saveIds.Add(u.Id);
                Studentes.Add(u);
            }
            await _repositoryGeneric.InsertRangeAsync(Studentes);
            await _repositoryGeneric.UnitOfWork.SaveEntitiesAsync();

            var filters = new List<Expression<Func<Student, bool>>> { e => saveIds.Contains(e.Id) };

            var result = await _repositoryGeneric.ToList(filters
                , e => e.OrderBy(x => x.Id).ThenBy(c => c.Name)
                , new PaginationQuery(), e => e.Class);

            Assert.True(result.Count == saveIds.Count);
            saveIds = saveIds.OrderBy(e => e).ToList();
            for (int i = 0; i < result.Count; i++)
            {
                Assert.True(result[i].Id == saveIds[i]);
                Assert.NotNull(result[i].Class);
            }
        }

        [Fact]
        public async Task GetAllAsync_Paging_without_Orderby_Success()
        {
            int start = 56, end = 60;
            List<long> saveIds = new List<long>();
            List<Student> Studentes = new List<Student>();
            for (int i = start; i < end; i++)
            {
                Student u = new Student();
                u.Class = new Class();
                u.Class.Id = Guid.NewGuid();
                u.Id = i;
                saveIds.Add(u.Id);
                Studentes.Add(u);
            }
            await _repositoryGeneric.InsertRangeAsync(Studentes);
            await _repositoryGeneric.UnitOfWork.SaveEntitiesAsync();

            var filters = new List<Expression<Func<Student, bool>>> { e => saveIds.Contains(e.Id) };

            var result = await _repositoryGeneric.ToList(filters
                , null
                , new PaginationQuery(), e => e.Class);

            Assert.True(result.Count == saveIds.Count);
            saveIds = saveIds.OrderByDescending(e => e).ToList();
            for (int i = 0; i < result.Count; i++)
            {
                Assert.True(result[i].Id == saveIds[i]);
                Assert.NotNull(result[i].Class);
            }
        }
    }
}