namespace MakeSimple.SharedKernel.Infrastructure.DTO
{
    using MakeSimple.SharedKernel.Contract;
    using MakeSimple.SharedKernel.Wrappers;
    using System.Collections.Generic;
    using System.Net;
    using System.Text.Json.Serialization;

    public class Response<TResponse> : ValueObject, IResponse<TResponse>
    {
        [JsonIgnore]
        public HttpStatusCode StatusCode { get; set; }

        public IError Error { get; set; }

#if NET5_0
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
#endif
        public TResponse Item { get; private set; }

        public Response(TResponse item)
        {
            StatusCode = HttpStatusCode.OK;
            Error = null;
            Item = item;
        }

        public Response(HttpStatusCode statusCode, IError error = null)
        {
            StatusCode = statusCode;
            Error = error;
            Item = default;
        }

        public void CopyFrom(IDataResult source)
        {
            StatusCode = source.StatusCode;
            Error = source.Error;
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return StatusCode;
            yield return Error;
            yield return Item;
        }
    }
}