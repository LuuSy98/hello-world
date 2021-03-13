using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows;

namespace MESInstaller.Models
{
    [JsonObject(MemberSerialization.OptIn)]
    public class MachineData
    {
        [JsonProperty]
        public List<string> BackupPaths { get; set; } = new List<string>();
        [JsonProperty]
        public List<string> ToCreatePaths { get; set; } = new List<string>();
        [JsonProperty]
        public List<FileDirector> FileDirectors { get; set; } = new List<FileDirector>();

        public string RootPath { get; set; }

        public bool AlreadyBackup { get; set; } = false;
    }
}
