using Newtonsoft.Json;
using System.Text.Json;

namespace MakeSimple.SharedKernel.Extensions
{
    public static class JsonElementExtentions
    {
        public static T ToObject<T>(this JsonElement element)
        {
            var json = element.GetRawText();
            return JsonConvert.DeserializeObject<T>(json);
        }

        public static T ToObject<T>(this JsonDocument document)
        {
            var json = document.RootElement.GetRawText();
            return JsonConvert.DeserializeObject<T>(json);
        }
    }
}