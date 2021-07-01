using MakeSimple.SharedKernel.Contract;
using System;

namespace MakeSimple.SharedKernel.Infrastructure.Exceptions
{
    public class UnhandledException : BaseException
    {
        public UnhandledException(IDataResult errorResult)
            : base(errorResult)
        { }

        public UnhandledException(IDataResult errorResult, Exception innerException)
            : base(errorResult, innerException)
        { }
    }
}