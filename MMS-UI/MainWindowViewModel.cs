using MmsEngine.Communications;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MMS_UI
{
    internal class MainWindowViewModel : BindableBase
    {
        private string _consoleData = string.Empty;
        public string ConsoleData
        {
            get => _consoleData;
            set => SetProperty(ref _consoleData, value);
        }
        private ConnectionState _status;
        public ConnectionState Status
        {
            get => _status;
            set => SetProperty(ref _status, value);
        }
    }
}
