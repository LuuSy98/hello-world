using System;
using System.Collections.Generic;
using System.Text;

namespace MESInstaller.Models
{
    public class BackupData
    {
        public string SelectedLine { get; set; }
        public string SelectedMachine { get; set; }
        public string IPString { get; set; }
        public string PathToLineList { get; set; }
        public string SubnetMask { get; set; }
        public string DefaultGateway { get; set; }
    }
}
