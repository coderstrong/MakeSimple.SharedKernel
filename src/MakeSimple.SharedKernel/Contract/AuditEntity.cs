namespace MakeSimple.SharedKernel.Contract
{
    public abstract class AuditEntityShared : EntityShared
    {
        public string CreatedBy { get; set; }

        public string ModifiedBy { get; set; }

        public string DeletedBy { get; set; }
    }

    public abstract class AuditEntity<T> : AuditEntityShared
    {
        public T Id { get; set; }
    }
}