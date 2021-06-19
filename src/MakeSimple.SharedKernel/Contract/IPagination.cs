namespace MakeSimple.SharedKernel.Contract
{
    using System.Collections.Generic;

    public interface IPagination<out T>
    {
        public T TotalItems { get; }
        public T CurrentPage { get; }
        public T PageSize { get; }
        public T TotalPages { get; }
        public T StartPage { get; }
        public T EndPage { get; }
        public T StartIndex { get; }
        public T EndIndex { get; }
        public IEnumerable<T> Pages { get; }
    }
}