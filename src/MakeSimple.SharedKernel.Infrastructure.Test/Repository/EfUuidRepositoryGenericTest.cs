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
    public class EfUuidRepositoryGenericTest
    {
        private readonly IRepositoryGeneric<MyDBContext, Course> _repositoryGeneric;

        public EfUuidRepositoryGenericTest()
        {
            var config = new MapperConfiguration(cfg => cfg.CreateMap<Course, ClassDto>().ReverseMap());

            _repositoryGeneric = new EfUuidRepositoryGeneric<MyDBContext, Course>(
                new MyDBContext(), SieveMock.Create(), new Mapper(config));
        }

        [Fact]
        public async Task Delete_SuccessAsync()
        {
            Course u = new Course();
            u.Id = Guid.NewGuid();

            _repositoryGeneric.Insert(u);
            await _repositoryGeneric.UnitOfWork.SaveEntitiesAsync();
            _repositoryGeneric.Delete(u.Id);

            var result = await _repositoryGeneric.UnitOfWork.SaveEntitiesAsync();

            Assert.True(result);
        }

        [Fact]
        public async Task Update_SuccessAsync()
        {
            Course u = new Course();
            u.Id = Guid.NewGuid();

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
            List<Guid> saveIds = new List<Guid>();
            List<Course> Classes = new List<Course>();
            for (int i = start; i < end; i++)
            {
                Course u = new Course();
                u.Id = Guid.NewGuid();
                saveIds.Add(u.Id);
                Classes.Add(u);
            }
            await _repositoryGeneric.InsertRangeAsync(Classes);
            await _repositoryGeneric.UnitOfWork.SaveEntitiesAsync();

            var filters = new List<Expression<Func<Course, bool>>> { e => saveIds.Contains(e.Id) };

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
            List<Guid> saveIds = new List<Guid>();
            List<Course> Classes = new List<Course>();
            for (int i = start; i < end; i++)
            {
                Course u = new Course();
                u.Id = Guid.NewGuid();
                u.Name = $"Name{i}";
                saveIds.Add(u.Id);
                Classes.Add(u);
            }
            await _repositoryGeneric.InsertRangeAsync(Classes);
            await _repositoryGeneric.UnitOfWork.SaveEntitiesAsync();

            var filters = new List<Expression<Func<Course, bool>>> { e => saveIds.Contains(e.Id) };

            var result = await _repositoryGeneric.ToListAsync<ClassDto>(new PaginationQuery(), filters: filters, expandFilters: "Name@=Name", expandSorts: "-Name");

            Assert.True(result.TotalItems == saveIds.Count);
            var idResults = result.Items.Select(e => e.Id).OrderBy(e => e).ToList();
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
            List<Guid> saveIds = new List<Guid>();
            List<Course> Classes = new List<Course>();
            for (int i = start; i < end; i++)
            {
                Course u = new Course();
                u.Id = Guid.NewGuid();
                saveIds.Add(u.Id);
                Classes.Add(u);
            }
            await _repositoryGeneric.InsertRangeAsync(Classes);
            await _repositoryGeneric.UnitOfWork.SaveEntitiesAsync();

            var filters = new List<Expression<Func<Course, bool>>> { e => e.Id == Guid.NewGuid() };

            var result = await _repositoryGeneric.ToListAsync(filters);

            Assert.True(result.Count == 0);
        }

        [Fact]
        public async Task GetAllAsync_WithMapper_NotFound_Success()
        {
            int start = 26, end = 30;
            List<Guid> saveIds = new List<Guid>();
            List<Course> Classes = new List<Course>();
            for (int i = start; i < end; i++)
            {
                Course u = new Course();
                u.Id = Guid.NewGuid();
                saveIds.Add(u.Id);
                Classes.Add(u);
            }
            await _repositoryGeneric.InsertRangeAsync(Classes);
            await _repositoryGeneric.UnitOfWork.SaveEntitiesAsync();

            var filters = new List<Expression<Func<Course, bool>>> { e => e.Id == Guid.NewGuid() };

            var result = await _repositoryGeneric.ToListAsync<ClassDto>(new PaginationQuery(), filters: filters);

            Assert.True(result.TotalItems == 0);
        }

        [Fact]
        public async Task GetFirstOrDefaultAsync_NotFound_Success()
        {
            int start = 31, end = 35;
            List<Guid> saveIds = new List<Guid>();
            List<Course> Classes = new List<Course>();
            for (int i = start; i < end; i++)
            {
                Course u = new Course();
                u.Id = Guid.NewGuid();
                saveIds.Add(u.Id);
                Classes.Add(u);
            }
            await _repositoryGeneric.InsertRangeAsync(Classes);
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
            List<Course> Classes = new List<Course>();
            for (int i = start; i < end; i++)
            {
                Course u = new Course();
                u.Id = Guid.NewGuid();
                saveIds.Add(u.Id);
                Classes.Add(u);
            }
            await _repositoryGeneric.InsertRangeAsync(Classes);
            await _repositoryGeneric.UnitOfWork.SaveEntitiesAsync();

            var result = await _repositoryGeneric.FirstOrDefaultAsync<ClassDto>(saveIds[2]);

            Assert.NotNull(result);
            Assert.Equal(result.Item.Id, saveIds[2]);
        }

        [Fact]
        public async Task GetFirstOrDefaultAsync_Full_Parram_Success()
        {
            int start = 146, end = 150;
            Dictionary<Guid, long> saveIds = new Dictionary<Guid, long>();
            List<Course> Classes = new List<Course>();
            for (int i = start; i < end; i++)
            {
                Course u = new Course();
                var ad = new Student();
                ad.Id = i;
                u.Students = new List<Student>()
                {
                    ad
                };
                u.Id = Guid.NewGuid();
                saveIds.Add(u.Id, i);
                Classes.Add(u);
            }
            await _repositoryGeneric.InsertRangeAsync(Classes);
            await _repositoryGeneric.UnitOfWork.SaveEntitiesAsync();

            var result = await _repositoryGeneric.FirstOrDefaultAsync(saveIds.First().Key, e => e.Students);

            Assert.NotNull(result);
            Assert.NotNull(result.Students);
            Assert.Equal(result.Id, saveIds.First().Key);
        }

        [Fact]
        public async Task GetAllAsync_Full_Parrams_Success()
        {
            int start = 151, end = 155;
            List<Guid> saveIds = new List<Guid>();
            List<Course> Classes = new List<Course>();
            for (int i = start; i < end; i++)
            {
                Course u = new Course();
                var ad = new Student();
                ad.Id = i;
                u.Students = new List<Student>()
                {
                    ad
                };
                u.Id = Guid.NewGuid();
                saveIds.Add(u.Id);
                Classes.Add(u);
            }
            await _repositoryGeneric.InsertRangeAsync(Classes);
            await _repositoryGeneric.UnitOfWork.SaveEntitiesAsync();

            var filters = new List<Expression<Func<Course, bool>>> { e => saveIds.Contains(e.Id) };

            var result = await _repositoryGeneric.ToListAsync(filters
                , e => e.OrderBy(x => x.Id).ThenBy(c => c.Name)
                , new PaginationQuery(), e => e.Students);

            Assert.True(result.Count == saveIds.Count);
            saveIds = saveIds.OrderBy(e => e).ToList();
            for (int i = 0; i < result.Count; i++)
            {
                Assert.True(result[i].Id == saveIds[i]);
                Assert.NotNull(result[i].Students);
            }
        }

        [Fact]
        public async Task GetAllAsync_Paging_without_Orderby_Success()
        {
            int start = 156, end = 160;
            List<Guid> saveIds = new List<Guid>();
            List<Course> Classes = new List<Course>();
            for (int i = start; i < end; i++)
            {
                Course u = new Course();
                var ad = new Student();
                ad.Id = i;
                u.Students = new List<Student>()
                {
                    ad
                };
                u.Id = Guid.NewGuid();
                saveIds.Add(u.Id);
                Classes.Add(u);
            }
            await _repositoryGeneric.InsertRangeAsync(Classes);
            await _repositoryGeneric.UnitOfWork.SaveEntitiesAsync();

            var filters = new List<Expression<Func<Course, bool>>> { e => saveIds.Contains(e.Id) };

            var result = await _repositoryGeneric.ToListAsync(filters
                , null
                , new PaginationQuery(), e => e.Students);

            Assert.True(result.Count == saveIds.Count);
            saveIds = saveIds.OrderByDescending(e => e).ToList();
            for (int i = 0; i < result.Count; i++)
            {
                Assert.True(result[i].Id == saveIds[i]);
                Assert.NotNull(result[i].Students);
            }
        }
    }
}