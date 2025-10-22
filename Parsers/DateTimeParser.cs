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
        public static bool TryParse(string s, out DateTime dt)
        {
            return DateTime.TryParseExact(s, "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture, DateTimeStyles.None, out dt);
        }

        public static DateTime Parse(string s)
        {
            if (TryParse(s, out DateTime dt))
                return dt;
            else
                throw new FormatException($"DateTime String:{s}");
        }
    }
}
