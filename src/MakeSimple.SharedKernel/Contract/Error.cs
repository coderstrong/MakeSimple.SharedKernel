using MakeSimple.SharedKernel.Helpers;
using System.Net;

namespace MakeSimple.SharedKernel.Contract
{
    public class Error
    {
        public string Code { get; }
        public string Message { get; }
        public HttpStatusCode StatusCode { get; set; }
        public string TraceId { get; }

        protected Error(string code, string message, HttpStatusCode statusCode)
        {
            Code = code;
            Message = message;
            StatusCode = statusCode;
            TraceId = UuuidHelper.GenerateId();
        }

        public static Error Create(string code, string message, HttpStatusCode statusCode)
        {
            return new Error(code, message, statusCode);
        }
    }
}