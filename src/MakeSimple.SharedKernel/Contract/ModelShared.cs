namespace MakeSimple.SharedKernel.Contract
{
    using System;

    public abstract class ModelShared
    {
        public DateTime CreatedAt { get; set; }
        public DateTime? ModifiedAt { get; set; }
        public int Status { get; set; }
    }
}