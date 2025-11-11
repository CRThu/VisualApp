using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using ScottPlot;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using VisualApp.Messages;
using VisualApp.Parsers;
using VisualApp.Services;

namespace VisualApp.ViewModel
{
    public enum SeriesType
    {
        Line
    }


    public partial class DataPlotTabVM : TabVMBase
    {
        private readonly DataService _dataService;
        private readonly SeriesType _type;

        [ObservableProperty]
        private ObservableCollection<string> dataKeys;

        [ObservableProperty]
        private bool isXDataEnabled;

        [ObservableProperty]
        private bool isXDataDateTime;

        [ObservableProperty]
        private string selectedXDataKey;

        [ObservableProperty]
        private string selectedYDataKey;

        [ObservableProperty]
        private string renderStatusText;

        [ObservableProperty]
        private Plot plot;

        public DataPlotTabVM(DataService ds, SeriesType type, string header)
        {
            Title = header;
            _dataService = ds;
            _type = type;
            plot = new();

            DataKeys = new();

            _dataService.PropertyChanged += OndataServicePropertyChanged;

            UpdateDataKeys(_dataService.Data);
        }

        private void OndataServicePropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(DataService.Data))
            {
                UpdateDataKeys(_dataService.Data);
            }
        }

        private void UpdateDataKeys(DataTable? data)
        {
            DataKeys.Clear();
            if (data != null)
            {
                foreach (DataColumn column in data.Columns)
                {
                    DataKeys.Add(column.ColumnName);
                }
            }
        }

        partial void OnIsXDataEnabledChanged(bool value) => ReRender();
        partial void OnIsXDataDateTimeChanged(bool value) => ReRender();
        partial void OnSelectedXDataKeyChanged(string value) => ReRender();
        partial void OnSelectedYDataKeyChanged(string value) => ReRender();

        private void ReRender()
        {
            try
            {
                RenderStatusText = "绘图状态: 成功";
                Plot.Clear();

                // 检查数据和选择的列
                if (_dataService.Data == null || _dataService.Data.Rows.Count == 0)
                {
                    RenderStatusText = "绘图错误: 数据容器为空";
                    return;
                }

                if (string.IsNullOrEmpty(SelectedYDataKey))
                {
                    RenderStatusText = "绘图错误: 数据y轴键名为空";
                    return;
                }

                // 获取 Y 轴数据
                double[] ys = GetNumericData(SelectedYDataKey);
                if (ys == null || ys.Length == 0)
                {
                    RenderStatusText = "绘图错误: 数据y轴数据为空或解析失败";
                    return;
                }

                double[] xs = null;
                DateTime[] dts = null;
                bool useDateTime = false;
                string xLabel = "索引";

                // 尝试获取 X 轴数据（自动降级）
                if (IsXDataEnabled && !string.IsNullOrEmpty(SelectedXDataKey))
                {
                    if (IsXDataDateTime)
                    {
                        // 尝试日期时间
                        dts = GetDateTimeData(SelectedXDataKey);
                        if (dts != null && dts.Length > 0)
                        {
                            useDateTime = true;
                            xLabel = SelectedXDataKey;
                        }
                        else
                        {
                            // 日期解析失败，降级为数值
                            RenderStatusText = "绘图错误: X轴日期解析失败，降级为数值";
                            xs = GetNumericData(SelectedXDataKey);
                            if (xs != null && xs.Length > 0)
                            {
                                xLabel = SelectedXDataKey;
                            }
                        }
                    }
                    else
                    {
                        // 尝试数值
                        xs = GetNumericData(SelectedXDataKey);
                        if (xs != null && xs.Length > 0)
                        {
                            xLabel = SelectedXDataKey;
                        }
                        else
                        {
                            RenderStatusText = "绘图错误: X轴数据解析失败，降级为索引";
                        }
                    }
                }

                // 如果 X 轴数据获取失败，降级为索引
                if (!useDateTime && (xs == null || xs.Length == 0))
                {
                    xs = Enumerable.Range(0, ys.Length).Select(i => (double)i).ToArray();
                    xLabel = "索引";
                }

                // 绘制图表
                if (useDateTime)
                {
                    // 确保数据长度一致
                    int minLength = Math.Min(dts.Length, ys.Length);
                    Array.Resize(ref dts, minLength);
                    Array.Resize(ref ys, minLength);

                    Plot.Add.Scatter(dts, ys);
                    Plot.Axes.Remove(Edge.Bottom);
                    Plot.Axes.DateTimeTicksBottom();
                }
                else
                {
                    // 确保数据长度一致
                    int minLength = Math.Min(xs.Length, ys.Length);
                    Array.Resize(ref xs, minLength);
                    Array.Resize(ref ys, minLength);

                    Plot.Add.Scatter(xs, ys);
                    Plot.Axes.Remove(Edge.Bottom);
                    Plot.Axes.NumericTicksBottom();
                }

                Plot.Axes.Bottom.Label.Text = xLabel;
                Plot.Axes.Bottom.Label.FontSize = 18;
                Plot.Axes.Left.Label.Text = SelectedYDataKey;
                Plot.Axes.Left.Label.FontSize = 18;
                Plot.Legend.IsVisible = true;
                Plot.Legend.FontSize = 18;
                Plot.Axes.AntiAlias(true);
                Plot.Axes.AutoScale();
                Plot.ScaleFactor = 1.25;
                Plot.Font.Automatic();

                WeakReferenceMessenger.Default.Send(PlotResetMessage.Instance);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"绘图错误: {ex.Message}");
            }
        }

        /// <summary>
        /// 从 DataTable 中获取数值数据
        /// </summary>
        private double[] GetNumericData(string columnName)
        {
            if (_dataService.Data == null || !_dataService.Data.Columns.Contains(columnName))
                return null;

            List<double> result = new List<double>();
            foreach (DataRow row in _dataService.Data.Rows)
            {
                string value = row[columnName]?.ToString();
                if (!string.IsNullOrWhiteSpace(value) && double.TryParse(value, out double d))
                {
                    result.Add(d);
                }
            }

            return result.ToArray();
        }

        /// <summary>
        /// 从 DataTable 中获取日期时间数据
        /// </summary>
        private DateTime[] GetDateTimeData(string columnName)
        {
            if (_dataService.Data == null || !_dataService.Data.Columns.Contains(columnName))
                return null;

            List<DateTime> result = new List<DateTime>();
            foreach (DataRow row in _dataService.Data.Rows)
            {
                string value = row[columnName]?.ToString();
                if (!string.IsNullOrWhiteSpace(value) && DateTimeParser.TryParse(value, out DateTime dt))
                {
                    result.Add(dt);
                }
            }

            return result.ToArray();
        }
    }
}
