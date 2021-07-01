using MakeSimple.SharedKernel.Contract;
using System;

namespace MakeSimple.SharedKernel.Infrastructure.Exceptions
{
    public class ForbiddenException : BaseException
    {
        public ForbiddenException(IDataResult errorResult)
            : base(errorResult)
        { }

        public ForbiddenException(IDataResult errorResult, Exception innerException)
            : base(errorResult, innerException)
        { }
    }
}