using MESInstaller.Helpers;
using MESInstaller.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Windows;
using System.Windows.Input;

namespace MESInstaller.ViewModels
{
    [JsonObject(MemberSerialization.OptIn)]
    public class MainWindowViewModel : PropertyChangedNotifier
    {
        #region Properties
        public List<string> LineList
        {
            get { return _LineList; }
            set
            {
                if (_LineList != value)
                {
                    _LineList = value;
                    OnPropertyChanged("LineList");
                }
            }
        }

        [JsonProperty]
        public string SelectedLine
        {
            get { return _SelectedLine; }
            set
            {
                _SelectedLine = value;
                OnPropertyChanged("SelectedLine");
                
                if (_SelectedLine == "VCM_LINE")
                {
                    MachineList = Define.VCM_MACHINE_LIST;
                }
                else if(_SelectedLine == "IR_LINE")
                {
                    MachineList = Define.IR_MACHINE_LIST;
                }
            }
        }

        public List<string> MachineList
        {
            get { return _MachineList; }
            set
            {
                _MachineList = value;
                OnPropertyChanged("MachineList");
            }
        }

        [JsonProperty]
        public string SelectedMachine
        {
            get { return _SelectedMachine; }
            set
            {
                _SelectedMachine = value;
                OnPropertyChanged("SelectedMachine");
            }
        }

        public string InputMachineNumber
        {
            get { return _InputMachineNumber; }
            set
            {
                _InputMachineNumber = value;
                OnPropertyChanged("SelectedMachineNumber");
            }
        }


        [JsonProperty]
        public string IPString
        {
            get { return _IPString; }
            set
            {
                _IPString = value;
                OnPropertyChanged("IPString");
            }
        }

        [JsonProperty]
        public string SubnetMask
        {
            get { return _SubnetMask; }
            set
            {
                _SubnetMask = value;
                OnPropertyChanged("SubnetMask");
            }
        }

        [JsonProperty]
        public string DefaultGateway
        {
            get { return _DefaultGateway; }
            set
            {
                _DefaultGateway = value;
                OnPropertyChanged("DefaultGateway");
            }
        }

        [JsonProperty]
        public string PathToLineList
        {
            get { return _PathToLineList; }
            set
            {
                _PathToLineList = value;
                OnPropertyChanged("PathToLineList");
            }
        }

        public bool DataBuildMode
        {
            get
            {
                return File.Exists(@"D:\DataBuildMode.txt");
            }
        }
        #endregion

        #region Command
        public ICommand InstallStartCommand
        {
            get
            {
                return _InstallStartCommand ?? (_InstallStartCommand = new RelayCommand<object>((o) =>
                {
                    if (InputValid() == false)
                    {
                        return;
                    }

                    PrintStartLog();

                    string basicContentPath = Path.Combine(PathToLineList, SelectedLine, "BasicContents");

                    BasicContentHelper.ContentCopy(basicContentPath);
                    NetworkHelper.SetIP(IPString, SubnetMask, DefaultGateway);
                    SpecificContentExecute();

                    PrintStopLog();
                }));
            }
        }

        public ICommand LineListPathBrowseCommand
        {
            get
            {
                return _LineListPathBrowseCommand ?? (_LineListPathBrowseCommand = new RelayCommand<object>((o) =>
                {
                    using (var dialog = new System.Windows.Forms.FolderBrowserDialog())
                    {
                        System.Windows.Forms.DialogResult result = dialog.ShowDialog();

                        if (result == System.Windows.Forms.DialogResult.OK)
                        {
                            PathToLineList = dialog.SelectedPath;
                        }
                    }
                }));
            }
        }

        public ICommand BackupDataCommand
        {
            get
            {
                return _BackupDataCommand ?? (_BackupDataCommand = new RelayCommand<object>((o) =>
                {
                    System.IO.File.WriteAllText(Define.BackupFilePath, JsonConvert.SerializeObject(this, Formatting.Indented));
                    Define.Logger.AddLog("MAIN", "Program End");
                    Define.Logger.Dispose();
                }));
            }
        }

        public ICommand NavigateCommand
        {
            get
            {
                return _NavigateCommand ?? (_NavigateCommand = new RelayCommand<object>((o) =>
                {
                    MessageBox.Show("HOI OI, CAI FORM LAM XONG CHUA?");
                }));
            }
        }

#endregion

#region Constructor
        public MainWindowViewModel()
        {
            Define.Logger.AddLog(new LogData { ProgressName = "MAIN", LogMessage = "Program Started" });

            LineList = Define.LINE_LIST;

            LoadBackupData();
        }
#endregion

#region Functions
        private bool InputValid()
        {
            if (string.IsNullOrEmpty(SelectedLine) || string.IsNullOrEmpty(SelectedMachine))
            {
                MessageBox.Show("Select line and machine first!");
                return false;
            }

            if (string.IsNullOrEmpty(PathToLineList))
            {
                MessageBox.Show("Select content directory first!");
                return false;
            }

            if (IPAddress.TryParse(IPString, out IPAddress ip) == false)
            {
                MessageBox.Show("IP wrong format!");
                return false;
            }

            if (string.IsNullOrEmpty(InputMachineNumber))
            {
                MessageBox.Show("Input machine number please!");
                return false;
            }

            return true;
        }

        private void LoadBackupData()
        {
            if (!File.Exists(Define.BackupFilePath))
            {
                return;
            }

            var backupData = JsonConvert.DeserializeObject<BackupData>(System.IO.File.ReadAllText(Define.BackupFilePath));

            SelectedLine = backupData.SelectedLine;
            SelectedMachine = backupData.SelectedMachine;
            IPString = backupData.IPString;
            PathToLineList = backupData.PathToLineList;
            SubnetMask = backupData.SubnetMask;
            DefaultGateway = backupData.DefaultGateway;
        }

        private void SpecificContentExecute()
        {
            string machineDataPath = Path.Combine(PathToLineList, SelectedLine, "Machines", SelectedMachine);
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
                machineData.Execute();
            }
        }

        private void PrintStartLog()
        {
            Define.Logger.ResetErrorCount();

            string startLog = 
                $"\n" +
                $"☛☛☛ START INSTALL MES\n" +
                $"☛☛☛ MACHINE : {SelectedLine}\\{SelectedMachine} #{InputMachineNumber}\n" +
                $"☛☛☛ IP      : {IPString} | {SubnetMask} | {DefaultGateway}\n";

            Define.Logger.AddLog("MAIN", startLog);
        }

        private void PrintStopLog()
        {
            int errorCount = Define.Logger.ErrorCount();
            string stopLog = "";
            if (errorCount > 0)
            {
                stopLog =
                    $"\n" +
                    $"☛☛☛ MES INSTALL DONE with {errorCount} ERROR(s)\n";

                Define.Logger.AddLog("MAIN", stopLog);

                MessageBox.Show($"{stopLog} Check error log for more details");
            }
            else
            {
                stopLog =
                    $"\n" +
                    $"☛☛☛ MES INSTALL SUCCESS\n";
                Define.Logger.AddLog("MAIN", stopLog);

                MessageBox.Show($"{stopLog} Check log for more details");
            }

        }
        #endregion

        #region Privates
        private List<string> _LineList;
        private string _SelectedLine;
        private List<string> _MachineList;
        private string _SelectedMachine;
        private string _IPString = "172.16.161.";
        private string _SubnetMask = "255.255.255.0";
        private string _DefaultGateway = "172.16.161.1";
        private string _PathToLineList;
        private string _InputMachineNumber;

        private ICommand _InstallStartCommand;
        private ICommand _LineListPathBrowseCommand;
        private ICommand _BackupDataCommand;
        private ICommand _NavigateCommand; 
#endregion
    }
}
