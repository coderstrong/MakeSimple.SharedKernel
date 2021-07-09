namespace MakeSimple.SharedKernel.Contract
{
    using System;

    public abstract class EntityShared
    {
        public DateTime CreatedAt { get; set; }
        public DateTime? ModifiedAt { get; set; }
        public int Status { get; set; }
    }

    public abstract class Entity<T> : EntityShared
    {
        public T Id { get; set; }
    }
}