namespace MakeSimple.SharedKernel.Exceptions
{
    using MakeSimple.SharedKernel.Contract;
    using System;
    using System.Net;
    using System.Runtime.Serialization;

    public class ValidationException : BaseException
    {
        public ValidationException(Error errorResult)
            : base(errorResult, HttpStatusCode.BadRequest)
        { }

        public ValidationException(Error errorResult, Exception innerException)
            : base(errorResult, HttpStatusCode.BadRequest, innerException)
        { }

        protected ValidationException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        { }
    }
}