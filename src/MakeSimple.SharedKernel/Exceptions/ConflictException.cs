namespace MakeSimple.SharedKernel.Exceptions
{
    using MakeSimple.SharedKernel.Contract;
    using System;
    using System.Net;
    using System.Runtime.Serialization;

    public class ConflictException : BaseException
    {
        public ConflictException(Error errorResult)
            : base(errorResult, HttpStatusCode.Conflict)
        { }

        public ConflictException(Error errorResult, Exception innerException)
            : base(errorResult, HttpStatusCode.Conflict, innerException)
        { }

        protected ConflictException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        { }
    }
}