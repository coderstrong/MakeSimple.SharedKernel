namespace MakeSimple.SharedKernel.Contract
{
    public interface IError
    {
        public string Code { get; }
        public string ErrorMessage { get; }
        public string TraceId { get; }
    }
}