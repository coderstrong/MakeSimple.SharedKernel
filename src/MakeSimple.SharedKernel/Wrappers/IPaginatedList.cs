namespace MakeSimple.SharedKernel.Wrappers
{
    using MakeSimple.SharedKernel.Contract;
    using System.Collections.Generic;

    public interface IPaginatedList<TResponse> : IDataResult
    {
        public int TotalItems { get;}
        public int CurrentPage { get;  }
        public int PageSize { get;  }
        public int TotalPages { get;  }
        public int StartPage { get;  }
        public int EndPage { get; }
        public int StartIndex { get;  }
        public int EndIndex { get; }
        public IEnumerable<int> Pages { get;  }
        public IEnumerable<TResponse> Items { get; }

    }
}