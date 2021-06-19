namespace MakeSimple.SharedKernel.Contract
{
    public interface IPaginationQuery
    {
        public int PageIndex { get; set; }

        public int PageSize { get; set; }

        public int Skip { get; }
    }
}