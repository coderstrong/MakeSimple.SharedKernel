using Sieve.Attributes;

namespace MakeSimple.SharedKernel.Contract
{
    public abstract class AuditableEntity : Entity
    {
        [Sieve(CanFilter = true, CanSort = true)]
        public string CreatedBy { get; set; }

        [Sieve(CanFilter = true, CanSort = true)]
        public string LastModifiedBy { get; set; }
    }
}