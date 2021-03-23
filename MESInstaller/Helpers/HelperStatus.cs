using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MESInstaller.Helpers
{
    public class HelperStatus
    {
        public static bool UseBasicContentHelper { get; set; } = true;
        public static bool UseNetworkHelper { get; set; } = true;
        public static bool UseSpecificContentHelper { get; set; } = true;

        public static bool DataBuildMode { get; set; } = false;
    }
}
