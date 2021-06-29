using System.Net;

namespace MakeSimple.SharedKernel.Contract
{
    public interface IDataResult
    { 
        HttpStatusCode StatusCode { get; set; }
        IError Error { get; set; }
        void CopyFrom(IDataResult source);
    }
}
