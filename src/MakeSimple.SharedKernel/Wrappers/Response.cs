namespace MakeSimple.SharedKernel.Wrappers
{
    using MakeSimple.SharedKernel.Contract;
    using System.Collections.Generic;
    using System.Net;
    public class Response<TResponse> : ValueObject, IDataResult<TResponse>
    {
        public HttpStatusCode StatusCode { get; set; }
        public TResponse Result { get; set; }

        protected Response(TResponse result, HttpStatusCode statusCode = HttpStatusCode.OK)
        {
            StatusCode = statusCode;
            Result = result;
        }

        public static Response<TResponse> Created(TResponse result, HttpStatusCode statusCode = HttpStatusCode.OK)
        {
            return new Response<TResponse>(result, statusCode);
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return StatusCode;
            yield return Result;
        }
    }
}
