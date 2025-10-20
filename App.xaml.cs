using System.Configuration;
using System.Data;
using System.Windows;
using VisualApp.ViewModel;

namespace VisualApp
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static ViewModelLocator Locator => (ViewModelLocator)App.Current.Resources["Locator"];

    }

}
