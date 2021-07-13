using MakeSimple.SharedKernel.Contract;
using MakeSimple.SharedKernel.Helpers;

namespace MakeSimple.SharedKernel.Infrastructure.DTO
{
    public class ErrorBase : IError
    {
        public string Code { get; }
        public string ErrorMessage { get; }
        public string TraceId { get; }

        public ErrorBase(string code, string errorMessage)
        {
            Code = code;
            ErrorMessage = errorMessage;
            TraceId = UuuidHelper.GenerateId();
        }
    }
}