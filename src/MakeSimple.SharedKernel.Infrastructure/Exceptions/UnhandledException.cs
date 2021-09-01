using MakeSimple.SharedKernel.Contract;
using System;
using System.Runtime.Serialization;

namespace MakeSimple.SharedKernel.Infrastructure.Exceptions
{
    public class UnhandledException : BaseException
    {
        public UnhandledException(Error errorResult)
            : base(errorResult)
        { }

        public UnhandledException(Error errorResult, Exception innerException)
            : base(errorResult, innerException)
        { }

        protected UnhandledException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        { }
    }
}