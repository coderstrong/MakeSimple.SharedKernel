

namespace MakeSimple.SharedKernel.Aspnet
{
    using MakeSimple.SharedKernel.Contract;
    using System.Net;
    using System.Text.Json.Serialization;
    public class DataResult<T>
    {
        [JsonIgnore]
        public HttpStatusCode StatusCode { get; set; }

        public IErrorCode Error { get; set; }

#if NET5_0
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
#endif
        public T Data { get; set; }
#if NET5_0
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
#endif
        public long? TotalRecords { get; set; }

        public void CopyFrom(DataResult<T> source)
        {
            StatusCode = source.StatusCode;
            Error = source.Error;
        }
    }
}
