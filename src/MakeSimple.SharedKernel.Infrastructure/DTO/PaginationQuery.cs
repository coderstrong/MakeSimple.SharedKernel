namespace MakeSimple.SharedKernel.DTO
{
    using MakeSimple.SharedKernel.Contract;
    public class PaginationQuery : IPaginationQuery
    {
        public int PageIndex { get; set; }

        public int PageSize { get; set; }

        public int Skip
        {
            get
            {
                if (PageIndex == 0)
                    return 0;
                return (PageIndex - 1) * PageSize;
            }
        }

        public PaginationQuery(int pageIndex = 1, int pageSize = 10)
        {
            PageIndex = pageIndex;
            PageSize = pageSize;
        }
    }
}
