using CommunityToolkit.Mvvm.ComponentModel;
using System.Data;

namespace VisualApp.ViewModel
{
    public partial class DataPreviewTabVM : TabVMBase
    {
        [ObservableProperty]
        private DataTable? data;

        public DataPreviewTabVM()
        {
            Title = "预览";
        }
    }
}