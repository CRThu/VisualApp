using ScottPlot.Colormaps;
using ScottPlot.PathStrategies;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace VisualApp.Parsers
{
    public static class JsonParser
    {
        public static DataTable Parse(string filePath)
        {
            DataTable dataTable = new DataTable();
            Dictionary<string, string[]> data = new Dictionary<string, string[]>();
            string jsonString = File.ReadAllText(filePath);
            using (JsonDocument doc = JsonDocument.Parse(jsonString))
            {
                var root = doc.RootElement;
                Flatten(root, data, "");
            }

            for (int i = 0; i < data.Values.Max(v => v.Length); i++)
            {
                dataTable.Rows.Add(dataTable.NewRow());
            }

            dataTable.Columns.AddRange(data.Keys.Select(c => new DataColumn(c, typeof(string))).ToArray());

            foreach (var kv in data)
            {
                for (int i = 0; i < kv.Value.Length; i++)
                {
                    dataTable.Rows[i][kv.Key] = kv.Value[i];
                }
            }
            return dataTable;
        }

        /// <summary>
        /// 将JSON元素递归地“压平”到一个字典中。
        /// </summary>
        private static void Flatten(JsonElement element, Dictionary<string, string[]> data, string prefix = "")
        {
            switch (element.ValueKind)
            {
                case JsonValueKind.Object:
                    foreach (var property in element.EnumerateObject())
                    {
                        string newPrefix = string.IsNullOrEmpty(prefix) ? property.Name : $"{prefix}::{property.Name}";
                        Flatten(property.Value, data, newPrefix);
                    }
                    break;
                case JsonValueKind.Array:
                    // 嵌套的数组直接转换为字符串
                    data[prefix] = element.EnumerateArray().Select(i => i.ToString()).ToArray();
                    break;
                default:
                    // 处理 String, Number, True, False, Null
                    data[prefix] = new string[] { element.ToString() };
                    break;
            }
        }

    }
}
