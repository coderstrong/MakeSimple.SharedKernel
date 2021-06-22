using MakeSimple.SharedKernel.Contract;
using System;

namespace MakeSimple.SharedKernel.Infrastructure.Exceptions
{
    public class MissingOrWrongConfigException : BaseException
    {
        public MissingOrWrongConfigException(IDataResult errorResult)
            : base(errorResult)
        { }

        public MissingOrWrongConfigException(IDataResult errorResult, Exception innerException)
            : base(errorResult, innerException)
        { }
    }
}
