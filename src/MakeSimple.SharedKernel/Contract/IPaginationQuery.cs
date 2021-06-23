namespace MakeSimple.SharedKernel.Contract
{
    public interface IPaginationQuery
    {
        public int PageNumber { get; set; }

        public int PageSize { get; set; }

        public int Skip { get; }
    }
}