namespace MakeSimple.SharedKernel.Contract
{
    using System;

    public abstract class Entity
    {
        public DateTime CreatedAt { get; set; }
        public DateTime? LastModifiedAt { get; set; }
    }
}