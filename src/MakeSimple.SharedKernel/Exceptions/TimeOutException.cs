namespace MakeSimple.SharedKernel.Exceptions
{
    using MakeSimple.SharedKernel.Contract;
    using System;
    using System.Net;
    using System.Runtime.Serialization;

    public class TimeOutException : BaseException
    {
        public TimeOutException(Error errorResult)
            : base(errorResult, HttpStatusCode.RequestTimeout)
        { }

        public TimeOutException(Error errorResult, Exception innerException)
            : base(errorResult, HttpStatusCode.RequestTimeout, innerException)
        { }

        protected TimeOutException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        { }
    }
}