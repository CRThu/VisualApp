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
    public static class ExcelParser
    {
        public static DataTable Parse(string filePath)
        {
            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);

            using (var stream = File.Open(filePath, FileMode.Open, FileAccess.Read))
            {
                using (var reader = ExcelReaderFactory.CreateReader(stream))
                {
                    var dataSet = reader.AsDataSet(new ExcelDataSetConfiguration()
                    {
                        ConfigureDataTable = (_) => new ExcelDataTableConfiguration()
                        {
                            UseHeaderRow = true
                        }
                    });

                    if (dataSet.Tables.Count == 0)
                    {
                        return new DataTable("Empty");
                    }

                    DataTable mergedData = new DataTable("MergedExcel");

                    int maxRows = 0;
                    foreach (DataTable table in dataSet.Tables)
                    {
                        maxRows = Math.Max(maxRows, table.Rows.Count);
                    }

                    for (int i = 0; i < maxRows; i++)
                    {
                        mergedData.Rows.Add(mergedData.NewRow());
                    }

                    foreach (DataTable sheet in dataSet.Tables)
                    {
                        foreach (DataColumn sourceCol in sheet.Columns)
                        {
                            // 使用 "::" 作为分隔符
                            string newColName = $"{sheet.TableName}::{sourceCol.ColumnName}";

                            if (mergedData.Columns.Contains(newColName))
                            {
                                newColName = $"{newColName}_{Guid.NewGuid().ToString("N").Substring(0, 4)}";
                            }

                            DataColumn newCol = new DataColumn(newColName, typeof(string));
                            mergedData.Columns.Add(newCol);

                            for (int i = 0; i < sheet.Rows.Count; i++)
                            {
                                mergedData.Rows[i][newCol] = sheet.Rows[i][sourceCol]?.ToString() ?? string.Empty;
                            }
                        }
                    }

                    return mergedData;
                }
            }
        }
    }
}
