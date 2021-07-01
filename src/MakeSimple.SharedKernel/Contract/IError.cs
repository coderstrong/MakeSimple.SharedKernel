namespace MakeSimple.SharedKernel.Contract
{
    public interface IError
    {
        public string Code { get; set; }
        public string ErrorMessage { get; set; }
        public string TraceId { get; }
    }
}