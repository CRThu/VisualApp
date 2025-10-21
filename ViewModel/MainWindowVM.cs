using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Win32;
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

        public string AppName => $"MeasureApp {Assembly.GetEntryAssembly()?.GetName().Version}";

        [ObservableProperty]
        private ObservableCollection<TabVMBase> tabItems;

        public IRelayCommand ImportDataCommand => _dataService.ImportDataCommand;

        public MainWindowVM(DataService ds, DataPreviewTabVM dpTab)
        {
            _dataService = ds;

            TabItems = new();
            TabItems.Add(dpTab);
        }

        [RelayCommand]
        public void AddPlotTab(string parameter)
        {
            switch (parameter)
            {
                case "Line":

                    break;
            }
        }
    }
}
