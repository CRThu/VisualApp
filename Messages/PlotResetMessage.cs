using CommunityToolkit.Mvvm.Messaging.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VisualApp.Messages
{
    public class PlotResetMessage
    {
        public static PlotResetMessage Instance { get; } = new PlotResetMessage();

        public PlotResetMessage()
        {
        }
    }
}
