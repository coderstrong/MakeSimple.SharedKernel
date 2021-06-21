namespace MakeSimple.SharedKernel.Contract
{
    public abstract class AuditEntity<T> : Entity<T>
    {
        public string CreatedBy { get; set; }

        public string ModifiedBy { get; set; }

        public string DeletedBy { get; set; }
    }
}