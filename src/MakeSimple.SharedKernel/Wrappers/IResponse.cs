namespace MakeSimple.SharedKernel.Wrappers
{
    using MakeSimple.SharedKernel.Contract;

    public interface IResponse<out TResponse> : IDataResult
    {
        public TResponse Item { get; }
    }
}