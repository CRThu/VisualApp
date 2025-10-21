using CommunityToolkit.Mvvm.ComponentModel;
using System.ComponentModel;
using System.Data;
using VisualApp.Services;

namespace VisualApp.ViewModel
{
    public partial class DataPreviewTabVM : TabVMBase
    {
        private readonly DataService _dataService;

        [ObservableProperty]
        private DataTable? previewData;

        public DataPreviewTabVM(DataService ds)
        {
            Title = "预览";
            _dataService = ds;

            _dataService.PropertyChanged += OndataServicePropertyChanged;

            UpdatePreviewData(_dataService.Data);
        }

        private void OndataServicePropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(DataService.Data))
            {
                UpdatePreviewData(_dataService.Data);
            }
        }

        private void UpdatePreviewData(DataTable? data)
        {
            PreviewData = data;
        }
    }
}