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
using VisualApp.Parsers;

namespace VisualApp.ViewModel
{
    public partial class MainWindowVM : BaseVM
    {
        public string AppName => $"MeasureApp {Assembly.GetEntryAssembly()?.GetName().Version}";

        [ObservableProperty]
        private ObservableCollection<TabVMBase> tabItems;

        public MainWindowVM()
        {
            TabItems = new();
            TabItems.Add(new DataPreviewTabVM());
        }

        [RelayCommand]
        private void ImportData()
        {
            try
            {
                //OpenFileDialog ofd = new OpenFileDialog();
                //if (ofd.ShowDialog() == true)
                //{
                //    string filePath = ofd.FileName;
                //    Data = ParserRouter.ParseFile(filePath);
                //}
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }
    }
}
