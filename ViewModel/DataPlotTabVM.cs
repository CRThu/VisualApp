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
                Plot.Clear();

                DateTime[] dts = null;
                double[] xs = null;
                double[] ys = null;


                if (IsXDataDateTime)
                {
                    string[] ss = new string[]
                    {
                            "2025-10-21 01:11:00",
                            "2025-10-22 02:22:00",
                            "2025-10-23 03:33:00",
                            "2025-10-24 04:44:00",
                            "2025-10-25 05:55:00",
                            "2025-10-26 06:00:00"
                    };
                    dts = ss.Select(s => DateTimeParser.Parse(s)).ToArray();

                    ys = Generate.RandomWalk(dts.Length);


                    Plot.Add.Scatter(dts, ys);

                    // setup the bottom axis to use DateTime ticks
                    Plot.Axes.Remove(Edge.Bottom);
                    var axis = Plot.Axes.DateTimeTicksBottom();
                }
                else
                {
                    xs = new double[] { 1, 2, 3, 4, 5 };
                    ys = Generate.RandomWalk(xs.Length);

                    Plot.Axes.Remove(Edge.Bottom);
                    var axis = Plot.Axes.NumericTicksBottom();

                    Plot.Add.Scatter(xs, ys);
                }

                Plot.Axes.Bottom.Label.Text = SelectedXDataKey;
                Plot.Axes.Left.Label.Text = SelectedYDataKey;
                Plot.Legend.IsVisible = true;

                Plot.Axes.AntiAlias(true);

                Plot.Axes.AutoScale();
                Plot.ScaleFactor = 1.5;

                WeakReferenceMessenger.Default.Send(PlotResetMessage.Instance);
                //Plot.SavePng("demo.png", 400, 300);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"{ex}");
            }
        }
    }
}
