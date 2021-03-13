using MESInstaller.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Windows;

namespace MESInstaller.Helpers
{
    public class SpecificContentHelper : Helper
    {
        public static bool DataBuildMode
        {
            get
            {
                return File.Exists(@"D:\DataBuildMode.txt");
            }
        }

        private string machineDataPath = "";

        public SpecificContentHelper(string machineDataPath, int percentageOfTotal)
        {
            PercentageOfTotal = percentageOfTotal;
            this.machineDataPath = machineDataPath;
        }

        protected override void Execute()
        {
            SpecificContentExecute();
        }

        public void SpecificContentExecute()
        {
            MachineData machineData = new MachineData();
            if (DataBuildMode)
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
            foreach (string folder in machineData.ToCreatePaths)
            {
                Directory.CreateDirectory(folder);
                Define.Logger.AddLog("DATA", $"Create \'{folder}\' Success!");
            }
        }

        public void MachineFolderBackup(MachineData machineData)
        {
            foreach (string folder in machineData.BackupPaths)
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

            machineData.AlreadyBackup = true;
        }

        public void CopyAndOverrideFile(MachineData machineData)
        {
            if (machineData.AlreadyBackup == false)
            {
                MessageBox.Show("Backup data first");
                return;
            }

            foreach (var fileDirector in machineData.FileDirectors)
            {
                string fileName = Path.GetFileName(fileDirector.Source_FilePath);

                string sourceFile = Path.Combine(machineData.RootPath, fileDirector.Source_FilePath);
                string destFile = Path.Combine(fileDirector.Destination_FolderPath, fileName);
                File.Copy(sourceFile, destFile, true);

                Define.Logger.AddLog("DATA", $"Copy \'{sourceFile}\' to \'{destFile}\' Success");
            }
        }
    }
}
