using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace VisualApp.ViewModel
{
    public partial class MainWindowVM : BaseVM
    {
        public string AppName => $"MeasureApp {Assembly.GetEntryAssembly()?.GetName().Version}";

    }
}
