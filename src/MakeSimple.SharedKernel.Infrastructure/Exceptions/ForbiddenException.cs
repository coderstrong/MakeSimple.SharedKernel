using MakeSimple.SharedKernel.Contract;
using System;
using System.Runtime.Serialization;

namespace MakeSimple.SharedKernel.Infrastructure.Exceptions
{
    public class ForbiddenException : BaseException
    {
        public ForbiddenException(Error errorResult)
            : base(errorResult)
        { }

        public ForbiddenException(Error errorResult, Exception innerException)
            : base(errorResult, innerException)
        { }

        protected ForbiddenException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        { }
    }
}