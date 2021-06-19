namespace MakeSimple.SharedKernel.Contract
{
    public interface IErrorCode
    {
        public string Code { get; set; }
        public string ErrorMessage { get; set; }
        public string TraceMessage { get; set; }
    }
}