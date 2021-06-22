using MakeSimple.SharedKernel.Contract;
using System;

namespace MakeSimple.SharedKernel.Infrastructure.Exceptions
{
    public abstract class BaseException : Exception
    {
        public IDataResult DataResult { get; private set; }

        protected BaseException(IDataResult errorResult)
            : base(errorResult.Error.ErrorMessage)
        {
            DataResult = errorResult;
        }

        protected BaseException(IDataResult errorResult, Exception innerException)
            : base(errorResult.Error.ErrorMessage, innerException)
        {
            DataResult = errorResult;
        }
    }
}
