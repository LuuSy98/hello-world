using MESInstaller.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows;

namespace MESInstaller.Helpers
{
    public class SpecificContentHelper : Helper
    {
        [DllImport("kernel32")]
        private static extern long WritePrivateProfileString(string section, string key, string val, string filePath);

        [DllImport("kernel32")]
        private static extern int GetPrivateProfileString(string section, string key, string def, StringBuilder retVal, int size, string filePath);

        private string machineDataPath = "";

        public SpecificContentHelper(string machineDataPath, int percentageOfTotal)
        {
            PercentageOfTotal = percentageOfTotal;
            this.machineDataPath = machineDataPath;
        }

        protected override void Execute()
        {
            if (HelperStatus.UseSpecificContentHelper == false) return;

            try
            {
                SpecificContentExecute();
            }
            catch(Exception ex)
            {
                Define.Logger.AddLog("SPECIFIC", $"{ex.Message}", IsError: true);
            }
        }

        public void SpecificContentExecute()
        {
            MachineData machineData = new MachineData();
            if (HelperStatus.DataBuildMode)
            {
                machineData.BackupPaths = new List<string>
                {
                    @"D:\TOP\UI",
                    @"D:\TOP\TASK",
                    @"D:\TOP\VCM_BONDING",
                    @"C:\Program Files\VCM_TEST"
                };

                foreach (string file in Directory.GetFiles(machineDataPath))
                {
                    if (file.EndsWith("MachineData.json"))
                    {
                        continue;
                    }

                    machineData.FileDirectors.Add(new FileDirector
                    {
                        Source_FilePath = file.Replace(machineDataPath, "").Replace(@"\", ""),
                        Destination_FolderPath = null
                    });
                }

                System.IO.File.WriteAllText(Path.Combine(machineDataPath, "MachineData.json"), JsonConvert.SerializeObject(machineData, Formatting.Indented));
            }
            else
            {
                machineData = JsonConvert.DeserializeObject<MachineData>(File.ReadAllText(Path.Combine(machineDataPath, "MachineData.json")));
                machineData.RootPath = machineDataPath;
                CompletedPercentage = 10;

                MachineFolderCreate(machineData);
                CompletedPercentage = 40;

                MachineFolderBackup(machineData);
                CompletedPercentage = 60;

                CopyAndOverrideFile(machineData);
            }

            CompletedPercentage = 100;
        }

        public void MachineFolderCreate(MachineData machineData)
        {
            if (machineData.ToCreatePaths == null) return;
            if (machineData.ToCreatePaths.Count == 0) return;

            foreach (string folder in machineData.ToCreatePaths)
            {
                Directory.CreateDirectory(folder);
                Define.Logger.AddLog("DATA", $"Create \'{folder}\' Success!");
            }
        }

        public void MachineFileCreate(MachineData machineData)
        {
            if (machineData.ToCreateFiles == null) return;
            if (machineData.ToCreateFiles.Count == 0) return;

            foreach (string file in machineData.ToCreateFiles)
            {
                // Create file if file not created
                if (File.Exists(file) == false)
                {
                    File.Create(file);
                    Define.Logger.AddLog("DATA", $"Create \'{file}\' Success!");
                }
            }
        }

        public void MachineFolderBackup(MachineData machineData)
        {
            if (machineData.BackupPaths == null) return;
            if (machineData.BackupPaths.Count == 0) return;

            foreach (string folder in machineData.BackupPaths)
            {
                if (!Directory.Exists(folder))
                {
                    continue;
                }

                string backupFolderName = $"{folder} - NO MES";

                if (Directory.Exists(backupFolderName))
                {
                    MessageBoxResult result = MessageBox.Show(
                        $"{backupFolderName} folder already exist.\nDo you want to create new backup folder?",
                        "Attention",
                        MessageBoxButton.YesNo);

                    if (result != MessageBoxResult.Yes)
                    {
                        machineData.AlreadyBackup = true;
                        return;
                    }
                    else
                    {
                        backupFolderName = $"{folder} - NO MES_{DateTime.Now.ToString("yyMMddHHmmss")}";
                    }
                }

                Directory.CreateDirectory(backupFolderName);

                //Now Create all of the directories
                foreach (string dirPath in Directory.GetDirectories(folder, "*", SearchOption.AllDirectories))
                {
                    Directory.CreateDirectory(dirPath.Replace(folder, backupFolderName));
                }

                //Copy all the files & Replaces any files with the same name
                foreach (string newPath in Directory.GetFiles(folder, "*.*", SearchOption.AllDirectories))
                {
                    File.Copy(newPath, newPath.Replace(folder, backupFolderName), true);
                }

                Define.Logger.AddLog("DATA", $"Backup \'{folder}\' to \'{backupFolderName}\' Success!");
            }

            machineData.AlreadyBackup = true;
        }

        public void CopyAndOverrideFile(MachineData machineData)
        {
            if (machineData.AlreadyBackup == false)
            {
                MessageBox.Show("Backup data first");
                return;
            }
            if (machineData.FileDirectors == null) return;
            if (machineData.FileDirectors.Count == 0) return;

            foreach (var fileDirector in machineData.FileDirectors)
            {
                string fileName = Path.GetFileName(fileDirector.Source_FilePath);

                string sourceFile = Path.Combine(machineData.RootPath, fileDirector.Source_FilePath);
                string destFile = Path.Combine(fileDirector.Destination_FolderPath, fileName);
                File.Copy(sourceFile, destFile, true);

                Define.Logger.AddLog("DATA", $"Copy \'{sourceFile}\' to \'{destFile}\' Success");
            }
        }

        public void CreateProfileString(MachineData machineData)
        {
            if (machineData.ProfileStrings == null) return;
            if (machineData.ProfileStrings.Count == 0) return;

            foreach(var profile in machineData.ProfileStrings)
            {
                WritePrivateProfileString(profile.Section, profile.Key, profile.Value, profile.FilePath);
                Define.Logger.AddLog("DATA", $"Write \"{profile.Section}\"{profile.Key}={profile.Value} to file {profile.FilePath} Success!");
            }
        }
    }
}
