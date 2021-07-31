using Sieve.Attributes;

namespace MakeSimple.SharedKernel.Contract
{
    public abstract class Entity<T> : ModelShared
    {
        [Sieve(CanFilter = true, CanSort = true)]
        public T Id { get; set; }
    }
}