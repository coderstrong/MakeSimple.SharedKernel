namespace MakeSimple.SharedKernel.Contract
{
    using System.Net;

    public interface IDataResult<T>
    {
        public string Version { get; set; }
        public HttpStatusCode StatusCode { get; set; }
        public T Result { get; set; }
    }
}
