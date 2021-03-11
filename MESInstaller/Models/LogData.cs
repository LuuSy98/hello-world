using System;
using System.Collections.Generic;
using System.Text;

namespace MESInstaller.Models
{
    public class LogData
    {
        public DateTime LogTime { get; } = DateTime.Now;
        public string ProgressName { get; set; }
        public bool IsError { get; set; } = false;
        public string LogMessage { get; set; }
    }
}
