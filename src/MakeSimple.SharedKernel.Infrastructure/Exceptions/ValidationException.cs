using MakeSimple.SharedKernel.Contract;
using System;

namespace MakeSimple.SharedKernel.Infrastructure.Exceptions
{
    /// <summary>
    /// No log when throw this exception
    /// </summary>
#pragma warning disable S3925 // "ISerializable" should be implemented correctly
    public class ValidationException : BaseException
#pragma warning restore S3925 // "ISerializable" should be implemented correctly
    {
        public ValidationException(IDataResult errorResult)
            : base(errorResult)
        { }

        public ValidationException(IDataResult errorResult, Exception innerException)
            : base(errorResult, innerException)
        { }
    }
}