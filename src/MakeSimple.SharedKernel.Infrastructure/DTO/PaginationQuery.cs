namespace MakeSimple.SharedKernel.Infrastructure.DTO
{
    using MakeSimple.SharedKernel.Contract;
    using System.Runtime.Serialization;
    using System.Text.Json.Serialization;

    /// <summary>
    /// Max page size support is 100 items, default 10 items per page
    /// </summary>
    public class PaginationQuery : IPaginationQuery
    {
        [JsonIgnore]
        [IgnoreDataMember]
        internal virtual int MaxPageSize { get; } = 100;

        [JsonIgnore]
        [IgnoreDataMember]
        internal virtual int DefaultPageSize { get; set; } = 10;

        public int PageNumber { get; set; }

        public int PageSize
        {
            get
            {
                return DefaultPageSize;
            }
            set
            {
                DefaultPageSize = value > MaxPageSize ? MaxPageSize : value;
            }
        }

        [JsonIgnore]
        [IgnoreDataMember]
        public int Skip
        {
            get
            {
                if (PageNumber == 0)
                    return 0;
                return (PageNumber - 1) * PageSize;
            }
        }
    }
}