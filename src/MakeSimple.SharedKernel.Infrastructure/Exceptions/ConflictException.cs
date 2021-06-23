using MakeSimple.SharedKernel.Contract;
using System;

namespace MakeSimple.SharedKernel.Infrastructure.Exceptions
{
    public class ConflictException : BaseException
    {
        public ConflictException(IDataResult errorResult)
            : base(errorResult)
        { }

        public ConflictException(IDataResult errorResult, Exception innerException)
            : base(errorResult, innerException)
        { }
    }
}
