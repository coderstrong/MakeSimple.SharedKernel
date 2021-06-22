using MakeSimple.SharedKernel.Contract;
using System;

namespace MakeSimple.SharedKernel.Infrastructure.Exceptions
{
    public class ValidateException : BaseException
    {
        public ValidateException(IDataResult errorResult)
            : base(errorResult)
        { }

        public ValidateException(IDataResult errorResult, Exception innerException)
            : base(errorResult, innerException)
        { }
    }
}
