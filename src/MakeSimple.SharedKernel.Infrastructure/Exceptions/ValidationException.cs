using MakeSimple.SharedKernel.Contract;
using System;

namespace MakeSimple.SharedKernel.Infrastructure.Exceptions
{
    public class ValidationException : BaseException
    {
        public ValidationException(IDataResult errorResult)
            : base(errorResult)
        { }

        public ValidationException(IDataResult errorResult, Exception innerException)
            : base(errorResult, innerException)
        { }
    }
}
