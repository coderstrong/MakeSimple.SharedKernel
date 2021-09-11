﻿namespace MakeSimple.SharedKernel.Contract
{
    /// <summary>
    /// Max page size support is 100 items, default 10 items per page, you can override and change it
    /// </summary>
    public abstract class PaginationQuery
    {
        protected virtual int MaxPageSize { get; } = 100;

        protected virtual int DefaultPageSize { get; set; } = 10;

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

        public virtual int TotalItems { get; set; }

        public int Skip()
        {
            if (PageNumber == 0)
                return 0;
            return (PageNumber - 1) * PageSize;
        }
    }
}