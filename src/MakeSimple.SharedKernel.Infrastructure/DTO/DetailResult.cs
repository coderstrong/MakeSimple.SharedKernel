namespace MakeSimple.SharedKernel.DTO
{
    using MakeSimple.SharedKernel.Contract;
    using System.Collections.Generic;
    using System.Net;
    using System.Text.Json.Serialization;

    public class DetailResult<TResponse> : ValueObject, IDataResult
    {
        [JsonIgnore]
        public HttpStatusCode StatusCode { get; set; }

        public IErrorCode Error { get; set; }

#if NET5_0
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
#endif
        public TResponse Item { get; set; }

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