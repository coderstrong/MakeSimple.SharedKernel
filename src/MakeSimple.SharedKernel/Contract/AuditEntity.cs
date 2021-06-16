namespace MakeSimple.SharedKernel.Contract
{
    using System;

    public abstract class AuditEntity
    {
        public long Id { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? ModifiedAt { get; set; }
        public DateTime? DeletedAt { get; set; }

        public string CreatedBy { get; set; }

        public string ModifiedBy { get; set; }

        public string DeletedBy { get; set; }

        public int Status { get; set; }
        public int State { get; set; }
    }
}
