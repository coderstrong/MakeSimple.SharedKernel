using System.Collections.Generic;

namespace MakeSimple.SharedKernel.Contract
{
    public interface IError
    {
        public string Code { get; }
        public string Message { get; }
        public Dictionary<string, string> Details { get; set; }
        public string TraceId { get; }
    }
}