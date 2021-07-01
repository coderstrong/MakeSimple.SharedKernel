using MakeSimple.SharedKernel.Contract;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sieve.Attributes;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace MakeSimple.SharedKernel.Infrastructure.Test.Mocks
{
    public class Address : AuditEntity<long>
    {
        [Sieve(CanFilter = true, CanSort = true)]
        public string Line1 { get; set; }

        [Sieve(CanFilter = true, CanSort = true)]
        public string Line2 { get; set; }

        public virtual User User { get; set; }
        public Guid? UserId { get; set; }
    }

    public class Course : Entity<Guid>
    {
        [Sieve(CanFilter = true, CanSort = true)]
        public string Name { get; set; }

        public virtual ICollection<Student> Students { get; set; }
    }

    public class MyDBContext : DbContext, IUnitOfWork
    {
        public DbSet<Address> Address { get; set; }

        public DbSet<Course> Classes { get; set; }

        public DbSet<Student> Students { get; set; }

        public DbSet<User> Users { get; set; }

        public string Uuid => Guid.NewGuid().ToString();

        public async Task<bool> SaveEntitiesAsync(CancellationToken cancellationToken = default)
        {
            return (await SaveChangesAsync(cancellationToken)) > 0;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseInMemoryDatabase("ForTest");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new UserEntityConfiguration());
            modelBuilder.ApplyConfiguration(new AddressEntityConfiguration());
            modelBuilder.ApplyConfiguration(new StudentEntityConfiguration());
            modelBuilder.ApplyConfiguration(new ClassEntityConfiguration());
        }

        //entities
    }

    public class Student : Entity<long>
    {
        public virtual Course Class { get; set; }
        public Guid? ClassId { get; set; }

        [Sieve(CanFilter = true, CanSort = true)]
        public string Name { get; set; }
    }

    public class User : AuditEntity<Guid>
    {
        public virtual ICollection<Address> Address { get; set; }
        public string Password { get; set; }

        [Sieve(CanFilter = true, CanSort = true)]
        public string UserName { get; set; }
    }

    #region Dto

    public class AddressDto
    {
        public long Id { get; set; }
        public string Line1 { get; set; }

        public string Line2 { get; set; }

        public User User { get; set; }
        public Guid? UserId { get; set; }
    }

    public class ClassDto : Entity<Guid>
    {
        public string Name { get; set; }
        public ICollection<Student> Students { get; set; }
    }

    public class StudentDto
    {
        public Course Class { get; set; }
        public Guid? ClassId { get; set; }
        public long Id { get; set; }
        public string Name { get; set; }
    }

    public class UserDto : AuditEntity<Guid>
    {
        public ICollection<Address> Address { get; set; }
        public string UserName { get; set; }
    }

    #endregion Dto

    #region Config

    public class AddressEntityConfiguration
        : IEntityTypeConfiguration<Address>
    {
        public void Configure(EntityTypeBuilder<Address> builder)
        {
            builder.HasKey(m => m.Id);
        }
    }

    public class ClassEntityConfiguration
        : IEntityTypeConfiguration<Course>
    {
        public void Configure(EntityTypeBuilder<Course> builder)
        {
            builder.HasKey(m => m.Id);
            builder.HasMany(e => e.Students).WithOne(e => e.Class).HasForeignKey(e => e.ClassId);
        }
    }

    public class StudentEntityConfiguration
        : IEntityTypeConfiguration<Student>
    {
        public void Configure(EntityTypeBuilder<Student> builder)
        {
            builder.HasKey(m => m.Id);
        }
    }

    public class UserEntityConfiguration
                    : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.HasKey(m => m.Id);
            builder.HasMany(e => e.Address).WithOne(e => e.User).HasForeignKey(e => e.UserId);
        }
    }

    #endregion Config
}