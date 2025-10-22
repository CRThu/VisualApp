using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Win32;
using ScottPlot;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using VisualApp.Parsers;
using VisualApp.Services;

namespace VisualApp.ViewModel
{
    public partial class MainWindowVM : BaseVM
    {
        private readonly DataService _dataService;
        private readonly Func<SeriesType, string, DataPlotTabVM> _plotFactory;

        public string AppName => $"VisualApp {Assembly.GetEntryAssembly()?.GetName().Version}";

        [ObservableProperty]
        private ObservableCollection<TabVMBase> tabItems;

        public IRelayCommand ImportDataCommand => _dataService.ImportDataCommand;

        public MainWindowVM(DataService ds, DataPreviewTabVM dpTab, Func<SeriesType, string, DataPlotTabVM> plotFactory)
        {
            _dataService = ds;
            _plotFactory = plotFactory;

            TabItems = new();
            TabItems.Add(dpTab);
        }

        [RelayCommand]
        public void AddPlotTab(string parameter)
        {
            switch (parameter)
            {
                case "Line":
                    TabItems.Add(_plotFactory(SeriesType.Line,"折线图"));
                    break;
            }
        }
    }
}
