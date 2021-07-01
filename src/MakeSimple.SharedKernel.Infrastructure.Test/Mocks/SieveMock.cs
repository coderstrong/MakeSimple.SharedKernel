using Microsoft.Extensions.Options;
using Sieve.Models;
using Sieve.Services;

namespace MakeSimple.SharedKernel.Infrastructure.Test.Mocks
{
    public static class SieveMock
    {
        public static SieveProcessor Create()
        {
            IOptions<SieveOptions> someOptions = Options.Create<SieveOptions>(new SieveOptions()
            {
                ThrowExceptions = true
            });
            return new SieveProcessor(someOptions);
        }
    }
}