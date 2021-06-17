using MakeSimple.SharedKernel.Contract;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace MakeSimple.SharedKernel.Infrastructure.Test.Mocks
{
    public class MyDBContext : DbContext, IUnitOfWork
    {
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseInMemoryDatabase("ForTest");
        }

        public string Uuid => Guid.NewGuid().ToString();

        public async Task<bool> SaveEntitiesAsync(CancellationToken cancellationToken = default)
        {
            return (await SaveChangesAsync(cancellationToken)) > 0;
        }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new UserEntityConfiguration());
            modelBuilder.ApplyConfiguration(new AddressEntityConfiguration());
            modelBuilder.ApplyConfiguration(new StudentEntityConfiguration());
            modelBuilder.ApplyConfiguration(new ClassEntityConfiguration());
        }
        //entities

        public DbSet<User> Users { get; set; }

        public DbSet<Address> Address { get; set; }

        public DbSet<Student> Students { get; set; }

        public DbSet<Class> Classes { get; set; }
    }

    public class User : AuditUuidEntity
    {
        public string UserName { get; set; }

        public string Password { get; set; }

        public virtual ICollection<Address> Address { get; set; }
    }

    public class Address : AuditEntity
    {
        public string Line1 { get; set; }

        public string Line2 { get; set; }

        public string UserId { get; set; }
        public virtual User User { get; set; }
    }

    public class Student : Entity
    {
        public string Name { get; set; }
        public string ClassId { get; set; }
        public virtual Class Class { get; set; }
    }

    public class Class : UuidEntity
    {
        public string Name { get; set; }
        public virtual ICollection<Student> Students { get; set; }
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

    public class AddressEntityConfiguration
        : IEntityTypeConfiguration<Address>
    {
        public void Configure(EntityTypeBuilder<Address> builder)
        {
            builder.HasKey(m => m.Id);
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

    public class ClassEntityConfiguration
        : IEntityTypeConfiguration<Class>
    {
        public void Configure(EntityTypeBuilder<Class> builder)
        {
            builder.HasKey(m => m.Id);
            builder.HasMany(e => e.Students).WithOne(e => e.Class).HasForeignKey(e => e.ClassId);
        }
    }
}
