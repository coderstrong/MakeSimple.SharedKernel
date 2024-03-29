﻿namespace MakeSimple.SharedKernel.Contract
{
    using System.Net;

    public interface IDataResult<T>
    {
        public HttpStatusCode StatusCode { get; set; }
        public T Result { get; set; }
    }
}