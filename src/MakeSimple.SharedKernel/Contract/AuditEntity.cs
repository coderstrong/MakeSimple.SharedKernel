namespace MakeSimple.SharedKernel.Contract
{
    public abstract class AuditEntity<T> : AuditModelShared
    {
        public T Id { get; set; }
    }
}