using MakeSimple.SharedKernel.Contract;
using System;
using System.Runtime.Serialization;

namespace MakeSimple.SharedKernel.Infrastructure.Exceptions
{
    public class ConflictException : BaseException
    {
        public ConflictException(Error errorResult)
            : base(errorResult)
        { }

        public ConflictException(Error errorResult, Exception innerException)
            : base(errorResult, innerException)
        { }

        protected ConflictException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        { }
    }
}