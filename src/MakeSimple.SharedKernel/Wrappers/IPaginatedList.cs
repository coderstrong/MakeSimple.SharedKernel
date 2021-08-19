namespace MakeSimple.SharedKernel.Wrappers
{
    using MakeSimple.SharedKernel.Contract;
    using System.Collections.Generic;

    public interface IPaginatedList<TResponse> : IDataResult
    {
        int TotalItems { get; }
        int CurrentPage { get; }
        int PageSize { get; }
        int TotalPages { get; }
        int StartPage { get; }
        int EndPage { get; }
        int StartIndex { get; }
        int EndIndex { get; }
        IEnumerable<int> Pages { get; }
        ICollection<TResponse> Items { get; }
    }
}