namespace MakeSimple.SharedKernel.Contract
{
    public abstract class Entity<T> : ModelShared
    {
        public T Id { get; set; }
    }
}