using System;
using System.Runtime.Serialization;

namespace MakeSimple.SharedKernel.Contract
{
    public abstract class BaseException : Exception
    {
        public Error Errors { get; private set; }

        protected BaseException(Error errors)
            : base(errors.Message)
        {
            Errors = errors;
        }

        protected BaseException(Error errors, Exception innerException)
            : base(errors.Message, innerException)
        {
            Errors = errors;
        }

        protected BaseException(SerializationInfo info, StreamingContext context) 
            : base(info, context)
        { }
    }
}