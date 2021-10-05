namespace MakeSimple.SharedKernel.Wrappers
{
    using MakeSimple.SharedKernel.Contract;
    using System;
    using System.Collections.Generic;
    using System.Net;

    public class PaginatedList<TResponse> : ValueObject, IDataResult<ICollection<TResponse>>
    {
        public PaginatedList()
        {
            Result = default;
        }

        protected PaginatedList(ICollection<TResponse> results
            , int totalItems
            , int currentPage = 1
            , int pageSize = 10)
        {
            // calculate total pages
            var totalPages = (int)Math.Ceiling((decimal)totalItems / (decimal)pageSize);

            // ensure current page isn't out of range
            if (currentPage < 1)
            {
                currentPage = 1;
            }
            else if (currentPage > totalPages)
            {
                currentPage = totalPages;
            }

            // update object instance with all pager properties required by the view
            TotalItems = totalItems;
            CurrentPage = currentPage;
            PageSize = pageSize;
            TotalPages = totalPages;
            Result = results;
            StatusCode = HttpStatusCode.OK;
        }

        public static PaginatedList<TResponse> Created(ICollection<TResponse> results
            , int totalItems
            , int currentPage = 1
            , int pageSize = 10)
        {
            return new PaginatedList<TResponse>(results, totalItems, currentPage, pageSize);
        }

        public string Version { get; set; }
        public HttpStatusCode StatusCode { get; set; }
        public ICollection<TResponse> Result { get; set; }
        public int TotalItems { get; private set; }
        public int CurrentPage { get; private set; }
        public int PageSize { get; private set; }
        public int TotalPages { get; private set; }
        
        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Version;
            yield return StatusCode;
            yield return Result;
            yield return TotalItems;
            yield return CurrentPage;
            yield return PageSize;
            yield return TotalPages;
        }
    }
}