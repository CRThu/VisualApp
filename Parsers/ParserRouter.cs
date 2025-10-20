using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VisualApp.Parsers
{
    public static class ParserRouter
    {
        public static DataTable ParseFile(string filePath)
        {
            string extension = Path.GetExtension(filePath).ToLower();

            switch (extension)
            {
                case ".json":
                    return JsonParser.Parse(filePath);
                case ".csv":
                    return CsvParser.Parse(filePath);
                case ".xls":
                case ".xlsx":
                    return ExcelParser.Parse(filePath);
                default:
                    throw new NotSupportedException($"文件类型 '{extension}' 不支持。");
            }
        }
    }
}
