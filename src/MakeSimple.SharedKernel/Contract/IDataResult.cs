using System.Net;

namespace MakeSimple.SharedKernel.Contract
{
    public interface IDataResult
    {
        public HttpStatusCode StatusCode { get; set; }
        public IErrorCode Error { get; set; }
        public void CopyFrom(IDataResult source);
    }
}
