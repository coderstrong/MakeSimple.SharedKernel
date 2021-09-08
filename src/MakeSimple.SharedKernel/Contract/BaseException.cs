namespace MakeSimple.SharedKernel.Contract
{
    using System;
    using System.Net;
    using System.Runtime.Serialization;

    public abstract class BaseException : Exception
    {
        public Error Errors { get; private set; }

        public HttpStatusCode Code { get; private set; }

        protected BaseException(Error errors, HttpStatusCode code)
            : base(errors.Message)
        {
            Errors = errors;
            Code = code;
        }

        protected BaseException(Error errors, HttpStatusCode code, Exception innerException)
            : base(errors.Message, innerException)
        {
            Errors = errors;
            Code = code;
        }

        protected BaseException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        { }
    }
}