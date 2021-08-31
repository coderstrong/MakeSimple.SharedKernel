namespace MakeSimple.SharedKernel.Contract
{
    using Sieve.Attributes;
    using System;

    public abstract class Entity
    {
        [Sieve(CanFilter = true, CanSort = true)]
        public DateTime CreatedAt { get; set; }

        [Sieve(CanFilter = true, CanSort = true)]
        public DateTime? LastModifiedAt { get; set; }
    }
}