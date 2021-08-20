using Sieve.Attributes;

namespace MakeSimple.SharedKernel.Contract
{
    public abstract class AuditEntity : ModelShared
    {
        [Sieve(CanFilter = true, CanSort = true)]
        public string CreatedBy { get; set; }

        [Sieve(CanFilter = true, CanSort = true)]
        public string ModifiedBy { get; set; }

        [Sieve(CanFilter = true, CanSort = true)]
        public string DeletedBy { get; set; }
    }
}