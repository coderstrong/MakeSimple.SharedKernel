using System;

namespace MakeSimple.SharedKernel.Helpers
{
    public static class UuuidHelper
    {
        public static string GenerateId(string guid = null)
        {
            return string.IsNullOrEmpty(guid) ? guid : Guid.NewGuid().ToString();
        }
    }
}