using System;

namespace MakeSimple.SharedKernel.Infrastructure.Helpers
{
    public static class UuuidHelper
    {
        public static string GenerateId(string guid = null)
        {
            return string.IsNullOrEmpty(guid) ? guid : Guid.NewGuid().ToString();
        }
    }
}
