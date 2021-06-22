using MakeSimple.SharedKernel.Infrastructure.Extensions;
using System;
using System.Security.Cryptography;

namespace MakeSimple.SharedKernel.Infrastructure.Helpers
{
    /// <summary>
    /// References: https://github.com/cloudnative-netcore/netcorekit/blob/d07dc233cc82f78a44c03b346eaddf460a33da2c/src/NetCoreKit.Utils/Helpers/CryptoRandomHelper.cs
    /// </summary>
    public static class CryptoRandomHelper
    {
        private static readonly RandomNumberGenerator _rng = RandomNumberGenerator.Create();

        public static byte[] CreateRandomBytes(int length)
        {
            var bytes = new byte[length];
            _rng.GetBytes(bytes);

            return bytes;
        }

        public static string CreateRandomKey(int length)
        {
            var bytes = new byte[length];
            _rng.GetBytes(bytes);

            return Convert.ToBase64String(CreateRandomBytes(length));
        }

        public static string CreateUniqueKey(int length = 8)
        {
            return CreateRandomBytes(length).ToHexString();
        }

        public static string CreateSeriesNumber(string prefix = "MSK")
        {
            return $"{prefix}{DateTime.Now.ToString("yyyyMMddHHmmss")}{CreateUniqueKey()}";
        }
    }
}