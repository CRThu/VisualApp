using ExcelDataReader;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VisualApp.Parsers
{
    public static class CsvParser
    {
        public static DataTable Parse(string filePath)
        {
            // 为了在.NET Core/.NET 5+中正常工作，需要注册编码提供程序
            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);

            using (var stream = File.Open(filePath, FileMode.Open, FileAccess.Read))
            {
                // 使用ExcelDataReader创建CSV读取器
                using (var reader = ExcelReaderFactory.CreateCsvReader(stream))
                {
                    var result = reader.AsDataSet(new ExcelDataSetConfiguration()
                    {
                        ConfigureDataTable = (_) => new ExcelDataTableConfiguration()
                        {
                            // 将第一行用作列名
                            UseHeaderRow = true
                        }
                    });

                    if (result.Tables.Count > 0)
                    {
                        DataTable sourceTable = result.Tables[0];
                        DataTable stringTable = new DataTable(sourceTable.TableName);

                        // 创建新的string类型列
                        foreach (DataColumn col in sourceTable.Columns)
                        {
                            stringTable.Columns.Add(col.ColumnName, typeof(string));
                        }

                        // 复制数据，并将所有内容转换为字符串
                        foreach (DataRow sourceRow in sourceTable.Rows)
                        {
                            DataRow destRow = stringTable.NewRow();
                            for (int i = 0; i < sourceTable.Columns.Count; i++)
                            {
                                destRow[i] = sourceRow[i]?.ToString() ?? string.Empty;
                            }
                            stringTable.Rows.Add(destRow);
                        }
                        return stringTable;
                    }
                    else
                    {
                        return new DataTable();
                    }
                }
            }
        }
    }
}
