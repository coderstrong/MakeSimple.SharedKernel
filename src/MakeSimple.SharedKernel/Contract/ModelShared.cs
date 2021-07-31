namespace MakeSimple.SharedKernel.Contract
{
    using Sieve.Attributes;
    using System;

    public abstract class ModelShared
    {
        [Sieve(CanFilter = true, CanSort = true)]
        public DateTime CreatedAt { get; set; }

        [Sieve(CanFilter = true, CanSort = true)]
        public DateTime? ModifiedAt { get; set; }

        [Sieve(CanFilter = true, CanSort = true)]
        public int Status { get; set; }
    }
}