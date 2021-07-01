using MakeSimple.SharedKernel.Contract;
using System;

namespace MakeSimple.SharedKernel.Infrastructure.Exceptions
{
    public class TryGetKeyNotFoundException : BaseException
    {
        public TryGetKeyNotFoundException(IDataResult errorResult)
            : base(errorResult)
        { }

        public TryGetKeyNotFoundException(IDataResult errorResult, Exception innerException)
            : base(errorResult, innerException)
        { }
    }
}