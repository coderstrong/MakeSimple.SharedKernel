using MakeSimple.SharedKernel.Contract;
using System;

namespace MakeSimple.SharedKernel.Infrastructure.Exceptions
{
#pragma warning disable S3925 // "ISerializable" should be implemented correctly

    public abstract class BaseException : Exception
#pragma warning restore S3925 // "ISerializable" should be implemented correctly
    {
        public IDataResult DataResult { get; private set; }

        protected BaseException(IDataResult errorResult)
            : base(errorResult.Error.Message)
        {
            DataResult = errorResult;
        }

        protected BaseException(IDataResult errorResult, Exception innerException)
            : base(errorResult.Error.Message, innerException)
        {
            DataResult = errorResult;
        }
    }
}