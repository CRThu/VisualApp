using DryIoc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VisualApp.ViewModel
{
    public class ViewModelLocator
    {
        private readonly Container container;

        public ViewModelLocator()
        {
            container = new Container();

            container.Register<MainWindowVM>(Reuse.Singleton);
        }

        public MainWindowVM MainWindow => container.Resolve<MainWindowVM>();
    }
}
