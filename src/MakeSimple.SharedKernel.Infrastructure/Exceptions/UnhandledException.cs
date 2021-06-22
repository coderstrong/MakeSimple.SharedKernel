using MakeSimple.SharedKernel.Contract;
using System;
using System.Text.Json;

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
