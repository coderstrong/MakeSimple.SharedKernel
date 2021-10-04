namespace MakeSimple.SharedKernel.Wrappers
{
    using MakeSimple.SharedKernel.Contract;
    using System.Collections.Generic;
    using System.Net;
    public class Response<TResponse> : ValueObject, IDataResult<TResponse>
    {
        public string Version { get; set; }
        public HttpStatusCode StatusCode { get; set; }
        public TResponse Result { get; set; }

        protected Response(string version, TResponse result, HttpStatusCode statusCode = HttpStatusCode.OK)
        {
            Version = version;
            StatusCode = statusCode;
            Result = result;
        }

        public static Response<TResponse> Created(string version, TResponse result, HttpStatusCode statusCode = HttpStatusCode.OK)
        {
            return new Response<TResponse>(version, result, statusCode);
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Version;
            yield return StatusCode;
            yield return Result;
            yield return Result;
        }
    }
}
