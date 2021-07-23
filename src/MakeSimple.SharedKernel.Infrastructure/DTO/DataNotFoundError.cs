namespace MakeSimple.SharedKernel.Infrastructure.DTO
{
    public class DataNotFoundError : ErrorBase
    {
        public DataNotFoundError(string condition)
            : base("DataNotFound", $"Data not found for {condition}")
        {
        }
    }
}