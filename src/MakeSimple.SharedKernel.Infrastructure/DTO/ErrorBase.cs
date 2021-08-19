using MakeSimple.SharedKernel.Contract;
using MakeSimple.SharedKernel.Helpers;
using System.Collections.Generic;

namespace MakeSimple.SharedKernel.Infrastructure.DTO
{
    public class ErrorBase : IError
    {
        public string Code { get; }
        public string Message { get; }
        public Dictionary<string, string> Details { get; set; }
        public string TraceId { get; }

        public ErrorBase(string code, string message, Dictionary<string, string> details = null)
        {
            Code = code;
            Message = message;
            Details = details;
            TraceId = UuuidHelper.GenerateId();
        }
    }
}