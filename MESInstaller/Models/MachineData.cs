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
        private bool alreadyBackup = false;

        public void Execute()
        {
            MachineFolderCreate();
            MachineFolderBackup();
            CopyAndOverrideFile();
        }

        public void MachineFolderCreate()
        {
            foreach (string folder in ToCreatePaths)
            {
                Directory.CreateDirectory(folder);
                Define.Logger.AddLog("DATA", $"Create \'{folder}\' Success!");
            }
        }

        public void MachineFolderBackup()
        {
            foreach (string folder in BackupPaths)
            {
                if (!Directory.Exists(folder))
                {
                    continue;
                }

                Directory.CreateDirectory(folder.Replace(folder, $"{folder} - NO MES"));

                //Now Create all of the directories
                foreach (string dirPath in Directory.GetDirectories(folder, "*", SearchOption.AllDirectories))
                {
                    Directory.CreateDirectory(dirPath.Replace(folder, $"{folder} - NO MES"));
                }

                //Copy all the files & Replaces any files with the same name
                foreach (string newPath in Directory.GetFiles(folder, "*.*", SearchOption.AllDirectories))
                {
                    File.Copy(newPath, newPath.Replace(folder, $"{folder} - NO MES"), true);
                }

                Define.Logger.AddLog("DATA", $"Backup \'{folder}\' to \'{folder.Replace(folder, $"{folder} - NO MES")}\' Success!");
            }

            alreadyBackup = true;
        }

        public void CopyAndOverrideFile()
        {
            if (alreadyBackup == false)
            {
                MessageBox.Show("Backup data first");
                return;
            }

            foreach (var fileDirector in FileDirectors)
            {
                string fileName = Path.GetFileName(fileDirector.Source_FilePath);

                string sourceFile = Path.Combine(RootPath, fileDirector.Source_FilePath);
                string destFile = Path.Combine(fileDirector.Destination_FolderPath, fileName);
                File.Copy(sourceFile, destFile, true);

                Define.Logger.AddLog("DATA", $"Copy \'{sourceFile}\' to \'{destFile}\' Success");
            }
        }
    }
}
