namespace MakeSimple.SharedKernel.Contract
{
    using MakeSimple.SharedKernel.Helpers;
    using System.Collections.Generic;

    public class Error
    {
        public string Code { get; }
        public string Message { get; }
        public string TraceId { get; }

        public Dictionary<string, string> Details { get; }

        protected Error(string code, string message, Dictionary<string, string> details = null)
        {
            Code = code;
            Message = message;
            TraceId = UuuidHelper.GenerateId();
            Details = details;
        }

        public static Error Created(string code, string message, Dictionary<string, string> details = null)
        {
            return new Error(code, message, details);
        }
    }
}