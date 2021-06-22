using MakeSimple.SharedKernel.Contract;
using MakeSimple.SharedKernel.Infrastructure.Helpers;

namespace MakeSimple.SharedKernel.Infrastructure.DTO
{
    public class ErrorBase : IError
    {
        public string Code { get; set; }
        public string ErrorMessage { get; set; }
        public string TraceId { get; private set; }

        public ErrorBase()
        {
            TraceId = UuuidHelper.GenerateId();
        }
    }
}
