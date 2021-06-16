namespace MakeSimple.SharedKernel.Contract
{
    using System;

    public abstract class UuidEntity
    {
        public string Id { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? ModifiedAt { get; set; }
        public DateTime? DeletedAt { get; set; }

        public int Status { get; set; }
        public int State { get; set; }
    }
}
