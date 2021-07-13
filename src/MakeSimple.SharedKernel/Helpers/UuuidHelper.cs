using System;

namespace MakeSimple.SharedKernel.Helpers
{
    public static class UuuidHelper
    {
        public static string GenerateId(string guid = null)
        {
            return guid ?? Guid.NewGuid().ToString();
        }
    }
}