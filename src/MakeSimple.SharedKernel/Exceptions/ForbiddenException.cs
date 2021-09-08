namespace MakeSimple.SharedKernel.Exceptions
{
    using MakeSimple.SharedKernel.Contract;
    using System;
    using System.Net;
    using System.Runtime.Serialization;
    public class ForbiddenException : BaseException
    {
        public ForbiddenException(Error errorResult)
            : base(errorResult, HttpStatusCode.Forbidden)
        { }

        public ForbiddenException(Error errorResult, Exception innerException)
            : base(errorResult, HttpStatusCode.Forbidden, innerException)
        { }

        protected ForbiddenException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        { }
    }
}