namespace MakeSimple.SharedKernel.Contract
{
    using System.Runtime.Serialization;
    /// <summary>
    /// Max page size support is 100 items, default 10 items per page, you can override and change it
    /// </summary>
    public abstract class PaginationQuery
    {
        [IgnoreDataMember]
        public virtual int MaxPageSize { get; protected set; } = 100;

        [IgnoreDataMember]
        public virtual int DefaultPageSize { get; protected set; } = 10;

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
