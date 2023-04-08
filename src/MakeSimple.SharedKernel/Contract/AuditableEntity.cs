namespace MakeSimple.SharedKernel.Contract
{
    public abstract class AuditableEntity : Entity
    {
        public string CreatedBy { get; set; }

        public string LastModifiedBy { get; set; }
    }
}