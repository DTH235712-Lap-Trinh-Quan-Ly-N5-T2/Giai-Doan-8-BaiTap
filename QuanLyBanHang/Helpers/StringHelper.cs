using System.Text.RegularExpressions;
using System.Text;
using System.Globalization;

namespace QuanLyBanHang // Đảm bảo namespace này khớp với project của bạn
{
    public static class StringHelper
    {
        public static string GenerateSlug(this string phrase)
        {
            if (string.IsNullOrEmpty(phrase)) return "";

            string str = phrase.ToLower();
            // Loại bỏ dấu tiếng Việt
            str = RemoveSign4VietnameseString(str);
            // Loại bỏ ký tự đặc biệt
            str = Regex.Replace(str, @"[^a-z0-9\s-]", "");
            // Thay khoảng trắng bằng dấu gạch ngang
            str = Regex.Replace(str, @"\s+", "-").Trim();
            return str;
        }

        private static string RemoveSign4VietnameseString(string str)
        {
            string[] VietnameseSigns = new string[]
            {
                "aAeEoOuUiIdDyY",
                "áàạảãâấầậẩẫăắằặẳẵ", "ÁÀẠẢÃÂẤẦẬẨẪĂẮẰẶẲẴ",
                "éèẹẻẽêếềệểễ", "ÉÈẸẺẼÊẾỀỆỂỄ",
                "óòọỏõôốồộổỗơớờợởỡ", "ÓÒỌỎÕÔỐỒỘỔỖƠỚỜỢỞỠ",
                "úùụủũưứừựửữ", "ÚÙỤỦŨƯỨỪỰỬỮ",
                "íìịỉĩ", "ÍÌỊỈĨ",
                "đ", "Đ",
                "ýỳỵỷỹ", "ÝỲỴỶỸ"
            };
            for (int i = 1; i < VietnameseSigns.Length; i++)
            {
                for (int j = 0; j < VietnameseSigns[i].Length; j++)
                    str = str.Replace(VietnameseSigns[i][j], VietnameseSigns[0][i - 1]);
            }
            return str;
        }
    }
}