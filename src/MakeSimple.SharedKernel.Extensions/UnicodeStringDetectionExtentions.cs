using System.Text.RegularExpressions;

namespace MakeSimple.SharedKernel.Extensions
{
    public static class UnicodeStringDetectionExtentions
    {
        public const string pattern = "[ÀÁÂÃÈÉÊÌÍÒÓÔÕÙÚĂĐĨŨƠàáâãèéêìíòóôõùúăđĩũơƯẠẢẤẦẨẪẬẮẰẲẴẶẸẺẼỀỂưạảấầẩẫậắằẳẵặẹẻẽềểỄỆỈỊỌỎỐỒỔỖỘỚỜỞỠỢỤỦỨỪễếệỉịọỏốồổỗộớờởỡợụủứừỬỮỰỲỴÝỶỸửữựỳỵỷỹ]";
        public static bool IsVietnamese(this string strInput)
        {
            return new Regex(pattern).IsMatch(strInput);
        }
    }
}
