﻿using AutoMapper;
using MakeSimple.SharedKernel.Contract;
using MakeSimple.SharedKernel.Infrastructure.Repository;
using MakeSimple.SharedKernel.Infrastructure.Test.Mocks;
using MakeSimple.SharedKernel.Wrappers;
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
        private readonly IRepository<MyDBContext, Student> _repositoryGeneric;
        private readonly IRepository<MyDBContext, User> _repositoryAuditGeneric;

        public EfRepositoryGenericTest()
        {
            var config = new MapperConfiguration(cfg => cfg.CreateMap<Student, StudentDto>().ReverseMap());
            var config2 = new MapperConfiguration(cfg => cfg.CreateMap<User, UserDto>().ReverseMap());
            _repositoryGeneric = new EfRepositoryGeneric<MyDBContext, Student>(
                new MyDBContext(), SieveMock.Create(), new Mapper(config));
            _repositoryAuditGeneric = new EfRepositoryGeneric<MyDBContext, User>(
                new MyDBContext(), SieveMock.Create(), new Mapper(config2));
        }

        [Fact]
        public async Task Delete_SuccessAsync()
        {
            Student u = new Student();
            u.Id = 1;

            _repositoryGeneric.Insert(u);
            await _repositoryGeneric.UnitOfWork.SaveEntitiesAsync();
            await _repositoryGeneric.DeleteAsync(u.Id);

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
            Assert.NotNull(u.LastModifiedAt);
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

            var result = await _repositoryGeneric.ToListAsync(filters);

            Assert.True(result.Count == saveIds.Count);
            var idResults = result.Select(e => e.Id).OrderBy(e => e).ToList();
            saveIds = saveIds.OrderBy(e => e).ToList();
            for (int i = 0; i < idResults.Count; i++)
            {
                Assert.True(idResults[i] == saveIds[i]);
            }
        }

        [Fact]
        public async Task GetAllAsync_Paginated_Success()
        {
            int start = 16, end = 20;
            List<long> saveIds = new List<long>();
            List<Student> Studentes = new List<Student>();
            for (int i = start; i < end; i++)
            {
                Student u = new Student();
                u.Id = i;
                u.Name = $"Name{i}";
                saveIds.Add(u.Id);
                Studentes.Add(u);
            }
            await _repositoryGeneric.InsertRangeAsync(Studentes);
            await _repositoryGeneric.UnitOfWork.SaveEntitiesAsync();

            var filters = new List<Expression<Func<Student, bool>>> { e => saveIds.Contains(e.Id) };

            var query = new PaginationQueryImp();
            query.PageNumber = 1;
            var result = await _repositoryGeneric.ToListAsync<StudentDto>(query, filters: filters, expandFilters: "Name@=Name", expandSorts: "-Name");

            Assert.True(query.TotalItems == saveIds.Count);
            var idResults = result.Select(e => e.Id).OrderBy(e => e).ToList();
            saveIds = saveIds.OrderBy(e => e).ToList();
            for (int i = 0; i < idResults.Count; i++)
            {
                Assert.True(idResults[i] == saveIds[i]);
            }

            var pagedResult = PaginatedList<StudentDto>.Created(result, query);
            Assert.True(pagedResult.TotalItems == query.TotalItems);
            Assert.True(pagedResult.PageSize == query.PageSize);
            Assert.True(pagedResult.CurrentPage == query.PageNumber);
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

            var result = await _repositoryGeneric.ToListAsync(filters);

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
            var query = new PaginationQueryImp();
            var result = await _repositoryGeneric.ToListAsync<StudentDto>(query, filters: filters);

            Assert.True(query.TotalItems == 0);
        }

        [Fact]
        public async Task GetFirstOrDefaultAsync_Found_Success()
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
        public async Task GetFirstOrDefaultAsync_WithMapper_Found_Success()
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

            var result = await _repositoryGeneric.FindAsync<StudentDto>(saveIds[2]);

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
                u.Class = new Course();
                u.Class.Id = Guid.NewGuid();
                u.Id = i;
                saveIds.Add(u.Id, u.Class.Id);
                Studentes.Add(u);
            }
            await _repositoryGeneric.InsertRangeAsync(Studentes);
            await _repositoryGeneric.UnitOfWork.SaveEntitiesAsync();

            var result = await _repositoryGeneric.FirstOrDefaultAsync(saveIds.First().Key);

            Assert.NotNull(result);
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
                u.Class = new Course();
                u.Class.Id = Guid.NewGuid();
                u.Id = i;
                saveIds.Add(u.Id);
                Studentes.Add(u);
            }
            await _repositoryGeneric.InsertRangeAsync(Studentes);
            await _repositoryGeneric.UnitOfWork.SaveEntitiesAsync();

            var filters = new List<Expression<Func<Student, bool>>> { e => saveIds.Contains(e.Id) };

            var result = await _repositoryGeneric.ToListAsync(filters
                , e => e.OrderBy(x => x.Id).ThenBy(c => c.Name)
                , new PaginationQueryImp(), includes: e => e.Class);

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
                u.Class = new Course();
                u.Class.Id = Guid.NewGuid();
                u.Id = i;
                saveIds.Add(u.Id);
                Studentes.Add(u);
            }
            await _repositoryGeneric.InsertRangeAsync(Studentes);
            await _repositoryGeneric.UnitOfWork.SaveEntitiesAsync();

            var filters = new List<Expression<Func<Student, bool>>> { e => saveIds.Contains(e.Id) };

            var result = await _repositoryGeneric.ToListAsync(filters
                , null
                , new PaginationQueryImp(), includes: e => e.Class);

            Assert.True(result.Count == saveIds.Count);
            result = result.OrderByDescending(e => e.Id).ToList();
            saveIds = saveIds.OrderByDescending(e => e).ToList();
            for (int i = 0; i < result.Count; i++)
            {
                Assert.True(result[i].Id == saveIds[i]);
                Assert.NotNull(result[i].Class);
            }
        }

        [Fact]
        public async Task GetFirstOrDefaultAsync_Linq_Found_Success()
        {
            int start = 61, end = 65;
            List<long> saveIds = new List<long>();
            List<Student> Studentes = new List<Student>();
            for (int i = start; i < end; i++)
            {
                Student u = new Student();
                u.Id = i;
                u.Class = new Course()
                {
                    Id = Guid.NewGuid()
                };
                saveIds.Add(u.Id);
                Studentes.Add(u);
            }
            await _repositoryGeneric.InsertRangeAsync(Studentes);
            await _repositoryGeneric.UnitOfWork.SaveEntitiesAsync();

            var result = await _repositoryGeneric.FirstOrDefaultAsync(e => e.Id == saveIds[3], includes: i => i.Class);

            Assert.NotNull(result);
            Assert.NotNull(result.Class);
            Assert.Equal(result.Id, saveIds[3]);
        }

        [Fact]
        public async Task GetFirstOrDefaultAsync_WithMapper_Linq_Found_Success()
        {
            int start = 66, end = 70;
            List<long> saveIds = new List<long>();
            List<Student> Studentes = new List<Student>();
            for (int i = start; i < end; i++)
            {
                Student u = new Student();
                u.Id = i;
                u.Class = new Course()
                {
                    Id = Guid.NewGuid()
                };
                saveIds.Add(u.Id);
                Studentes.Add(u);
            }
            await _repositoryGeneric.InsertRangeAsync(Studentes);
            await _repositoryGeneric.UnitOfWork.SaveEntitiesAsync();

            var result = await _repositoryGeneric.FindAsync<StudentDto>(e => e.Id == saveIds[2], includes: i => i.Class);

            Assert.NotNull(result);
            Assert.NotNull(result.Class);
            Assert.Equal(result.Id, saveIds[2]);
        }

        [Fact]
        public async Task GetFirstOrDefaultAsync_linq_Full_Parram_Success()
        {
            int start = 71, end = 75;
            Dictionary<long, Guid> saveIds = new Dictionary<long, Guid>();
            List<Student> Studentes = new List<Student>();
            for (int i = start; i < end; i++)
            {
                Student u = new Student();
                u.Class = new Course();
                u.Class.Id = Guid.NewGuid();
                u.Id = i;
                saveIds.Add(u.Id, u.Class.Id);
                Studentes.Add(u);
            }
            await _repositoryGeneric.InsertRangeAsync(Studentes);
            await _repositoryGeneric.UnitOfWork.SaveEntitiesAsync();

            var result = await _repositoryGeneric.FirstOrDefaultAsync(e => e.Id == saveIds.First().Key, includes: i => i.Class);

            Assert.NotNull(result);
            Assert.NotNull(result.Class);
            Assert.Equal(result.Id, saveIds.First().Key);
        }

        [Fact]
        public async Task GetFirstOrDefaultAsync_linq_Full_Parram_NotFound_Success()
        {
            int start = 76, end = 80;
            Dictionary<long, Guid> saveIds = new Dictionary<long, Guid>();
            List<Student> Studentes = new List<Student>();
            for (int i = start; i < end; i++)
            {
                Student u = new Student();
                u.Class = new Course();
                u.Class.Id = Guid.NewGuid();
                u.Id = i;
                saveIds.Add(u.Id, u.Class.Id);
                Studentes.Add(u);
            }
            await _repositoryGeneric.InsertRangeAsync(Studentes);
            await _repositoryGeneric.UnitOfWork.SaveEntitiesAsync();

            var result = await _repositoryGeneric.FirstOrDefaultAsync(e => e.Id == 10000, includes: i => i.Class);

            Assert.Null(result);
        }

        // Audit
        [Fact]
        public async Task Delete_Audit_SuccessAsync()
        {
            User u = new User();
            u.Id = Guid.NewGuid();

            _repositoryAuditGeneric.Insert(u);
            await _repositoryAuditGeneric.UnitOfWork.SaveEntitiesAsync();
            await _repositoryAuditGeneric.DeleteAsync(u.Id);

            var result = await _repositoryAuditGeneric.UnitOfWork.SaveEntitiesAsync();

            Assert.True(result);
        }

        [Fact]
        public async Task Update_Audit_SuccessAsync()
        {
            User u = new User();
            u.Id = Guid.NewGuid();

            _repositoryAuditGeneric.Insert(u);
            await _repositoryAuditGeneric.UnitOfWork.SaveEntitiesAsync();
            u.UserName = "Test";
            _repositoryAuditGeneric.Update(u);

            var result = await _repositoryAuditGeneric.UnitOfWork.SaveEntitiesAsync();
            var dataFromDb = await _repositoryAuditGeneric.FirstOrDefaultAsync(u.Id);
            Assert.True(result);
            Assert.NotNull(u.LastModifiedAt);
            Assert.NotNull(u.LastModifiedBy);
            Assert.Equal(dataFromDb.UserName, u.UserName);
        }

        [Fact]
        public async Task GetAllAsync_Audit_Success()
        {
            int start = 10, end = 15;
            List<Guid> saveIds = new List<Guid>();
            List<User> Studentes = new List<User>();
            for (int i = start; i < end; i++)
            {
                User u = new User();
                u.Id = Guid.NewGuid();
                saveIds.Add(u.Id);
                Studentes.Add(u);
            }
            await _repositoryAuditGeneric.InsertRangeAsync(Studentes);
            await _repositoryAuditGeneric.UnitOfWork.SaveEntitiesAsync();

            var filters = new List<Expression<Func<User, bool>>> { e => saveIds.Contains(e.Id) };

            var result = await _repositoryAuditGeneric.ToListAsync(filters);

            Assert.True(result.Count == saveIds.Count);
            var idResults = result.Select(e => e.Id).OrderBy(e => e).ToList();
            saveIds = saveIds.OrderBy(e => e).ToList();
            Assert.DoesNotContain(result, e => string.IsNullOrEmpty(e.CreatedBy));
            for (int i = 0; i < idResults.Count; i++)
            {
                Assert.True(idResults[i] == saveIds[i]);
            }
        }

        [Fact]
        public async Task GetAllAsync_Audit_Paginated_Success()
        {
            int start = 10, end = 15;
            List<Guid> saveIds = new List<Guid>();
            List<User> Studentes = new List<User>();
            for (int i = start; i < end; i++)
            {
                User u = new User();
                u.Id = Guid.NewGuid();
                u.UserName = "UserName";
                saveIds.Add(u.Id);
                Studentes.Add(u);
            }
            await _repositoryAuditGeneric.InsertRangeAsync(Studentes);
            await _repositoryAuditGeneric.UnitOfWork.SaveEntitiesAsync();

            var filters = new List<Expression<Func<User, bool>>> { e => saveIds.Contains(e.Id) };
            var query = new PaginationQueryImp();
            var result = await _repositoryAuditGeneric.ToListAsync<UserDto>(query, filters: filters, expandFilters: "UserName@=UserName", expandSorts: "-UserName");

            Assert.True(query.TotalItems == saveIds.Count);
            var idResults = result.Select(e => e.Id).OrderBy(e => e).ToList();
            saveIds = saveIds.OrderBy(e => e).ToList();
            for (int i = 0; i < idResults.Count; i++)
            {
                Assert.True(idResults[i] == saveIds[i]);
            }
        }
    }
}