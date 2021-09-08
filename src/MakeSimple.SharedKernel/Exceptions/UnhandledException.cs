namespace MakeSimple.SharedKernel.Exceptions
{
    using MakeSimple.SharedKernel.Contract;
    using System;
    using System.Net;
    using System.Runtime.Serialization;

    public class UnhandledException : BaseException
    {
        public UnhandledException(Error errorResult)
            : base(errorResult, HttpStatusCode.InternalServerError)
        { }

        public UnhandledException(Error errorResult, Exception innerException)
            : base(errorResult, HttpStatusCode.InternalServerError, innerException)
        { }

        protected UnhandledException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        { }
    }
}