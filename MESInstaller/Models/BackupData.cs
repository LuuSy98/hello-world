using System;
using System.Collections.Generic;
using System.Text;

namespace MESInstaller.Models
{
    public class BackupData
    {
        public string SelectedLine { get; set; }
        public string SelectedMachine { get; set; }
        public IPData IPInfo { get; set; } = new IPData();
        public string PathToLineList { get; set; }
        public bool UseBasicContentHelper { get; set; }
        public bool UseNetworkHelper { get; set; }
        public bool UseSpecificContentHelper { get; set; }
    }
}
