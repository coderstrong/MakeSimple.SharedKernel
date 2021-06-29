using Serilog;
using System;
using System.IO;
using System.Threading.Tasks;

namespace MakeSimple.SharedKernel.Infrastructure.Extensions
{
    public static class StreamExtentions
    {
        public static async Task<string> ReadToEndBufferingAsync(this Stream stream)
        {
            string result = string.Empty;

            try
            {
                stream.Seek(0, SeekOrigin.Begin);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Can't rewind stream.");
            }

            using (var reader = new StreamReader(stream))
            {
                result = await reader.ReadToEndAsync();
                stream.Position = 0;
            }

            return result;
        }
    }
}
