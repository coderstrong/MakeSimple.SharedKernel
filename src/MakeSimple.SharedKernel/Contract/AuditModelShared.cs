namespace MakeSimple.SharedKernel.Contract
{
    public abstract class AuditModelShared : ModelShared
    {
        public string CreatedBy { get; set; }

        public string ModifiedBy { get; set; }

        public string DeletedBy { get; set; }
    }
}