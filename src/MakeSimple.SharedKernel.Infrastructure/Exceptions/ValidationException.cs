using MakeSimple.SharedKernel.Contract;
using System;
using System.Runtime.Serialization;

namespace MakeSimple.SharedKernel.Infrastructure.Exceptions
{
    public class ValidationException : BaseException
    {
        public ValidationException(Error errorResult)
            : base(errorResult)
        { }

        public ValidationException(Error errorResult, Exception innerException)
            : base(errorResult, innerException)
        { }

        protected ValidationException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        { }
    }
}