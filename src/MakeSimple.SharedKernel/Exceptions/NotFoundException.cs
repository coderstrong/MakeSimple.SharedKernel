namespace MakeSimple.SharedKernel.Exceptions
{
    using MakeSimple.SharedKernel.Contract;
    using System;
    using System.Net;
    using System.Runtime.Serialization;

    public class NotFoundException : BaseException
    {
        public NotFoundException(Error errorResult)
            : base(errorResult, HttpStatusCode.NotFound)
        { }

        public NotFoundException(Error errorResult, Exception innerException)
            : base(errorResult, HttpStatusCode.NotFound, innerException)
        { }

        protected NotFoundException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        { }
    }
}