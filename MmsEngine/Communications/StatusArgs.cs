using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MmsEngine.Communications
{
    public class StatusArgs : EventArgs
    {
        public ConnectionState State { get; set; }
    }
}
