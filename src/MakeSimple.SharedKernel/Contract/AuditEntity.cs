using Sieve.Attributes;

namespace MakeSimple.SharedKernel.Contract
{
    public abstract class AuditEntity<T> : AuditModelShared
    {
        [Sieve(CanFilter = true, CanSort = true)]
        public T Id { get; set; }
    }
}