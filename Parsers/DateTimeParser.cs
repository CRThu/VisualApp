using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VisualApp.Parsers
{
    public static class DateTimeParser
    {
        // 常见日期时间格式列表
        private static readonly string[] DateTimeFormats = new[]
        {
            // 标准格式（带短横线）
            "yyyy-MM-dd HH:mm:ss.fff",
            "yyyy-MM-dd HH:mm:ss",
            "yyyy-MM-dd HH:mm",
            "yyyy-MM-dd",
            
            // 斜杠分隔
            "yyyy/MM/dd HH:mm:ss.fff",
            "yyyy/MM/dd HH:mm:ss",
            "yyyy/MM/dd HH:mm",
            "yyyy/MM/dd",
            
            // ISO 8601
            "yyyy-MM-ddTHH:mm:ss.fff",
            "yyyy-MM-ddTHH:mm:ss",
            "yyyy-MM-ddTHH:mm:ss.fffZ",
            "yyyy-MM-ddTHH:mm:ssZ",
            
            // 无分隔符格式
            "yyyyMMddHHmmssfff",     // 20251001112233000
            "yyyyMMddHHmmss",        // 20251001112233
            "yyyyMMdd",              // 20251001
            
            // 中文格式
            "yyyy年MM月dd日 HH:mm:ss",
            "yyyy年MM月dd日"
        };

        public static bool TryParse(string s, out DateTime dt)
        {
            dt = default;

            if (string.IsNullOrWhiteSpace(s))
                return false;

            s = s.Trim();

            // 1. 优先尝试自定义格式（精确匹配）
            if (DateTime.TryParseExact(s, DateTimeFormats, CultureInfo.InvariantCulture, DateTimeStyles.None, out dt))
                return true;

            // 2. 尝试标准解析（作为备选）
            if (DateTime.TryParse(s, CultureInfo.InvariantCulture, DateTimeStyles.None, out dt))
                return true;

            // 3. 尝试 Unix 时间戳（秒）
            if (long.TryParse(s, out long timestamp))
            {
                // Unix 时间戳范围检查（1970-2100年）
                if (timestamp >= 0 && timestamp <= 4102444800)
                {
                    dt = DateTimeOffset.FromUnixTimeSeconds(timestamp).DateTime;
                    return true;
                }

                // 毫秒时间戳
                if (timestamp >= 0 && timestamp <= 4102444800000)
                {
                    dt = DateTimeOffset.FromUnixTimeMilliseconds(timestamp).DateTime;
                    return true;
                }
            }

            return false;
        }

        public static DateTime Parse(string s)
        {
            if (TryParse(s, out DateTime dt))
                return dt;
            else
                throw new FormatException($"无法解析日期时间字符串: {s}");
        }
    }
}
