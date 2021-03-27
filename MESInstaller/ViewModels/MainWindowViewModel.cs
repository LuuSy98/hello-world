using MESInstaller.Helpers;
using MESInstaller.Models;
using MESInstaller.Views;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
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
        public IPData IPInfo
        {
            get { return _IPInfo; }
            set
            {
                _IPInfo = value;
                OnPropertyChanged("IPInfo");
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

        public string MachineDataPath
        {
            get
            {
                return Path.Combine(PathToLineList, SelectedLine, "Machines", SelectedMachine);
            }
        }

        public string BasicContentPath
        {
            get
            {
                return Path.Combine(PathToLineList, SelectedLine, "BasicContents");
            }
        }

        public int CompletedPercentage
        {
            get { return _CompletedPercentage; }
            set
            {
                if (_CompletedPercentage != value)
                {
                    _CompletedPercentage = value;
                    OnPropertyChanged("CompletedPercentage");
                }
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

                    CompletedPercentage = 0;

                    IHelper specificContentHelper = new SpecificContentHelper(MachineDataPath, 35);
                    IHelper basicContentHelper = new BasicContentHelper(BasicContentPath, 35);
                    IHelper networkHelper = new NetworkHelper(IPInfo, 30);

                    networkHelper.Run();
                    basicContentHelper.Run();
                    specificContentHelper.Run();

                    Task task = new Task(() => {
                        while(!specificContentHelper.IsDone || !basicContentHelper.IsDone || !networkHelper.IsDone)
                        {
                            CompletedPercentage = 
                                  specificContentHelper.PercentageOfTotal * specificContentHelper.CompletedPercentage / 100
                                + basicContentHelper.PercentageOfTotal * basicContentHelper.CompletedPercentage / 100
                                + networkHelper.PercentageOfTotal * networkHelper.CompletedPercentage / 100;

                            Thread.Sleep(100);
                        }

                        CompletedPercentage = 100;

                        PrintStopLog();
                    });

                    task.Start();
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
                    var backupData = new BackupData {
                        SelectedLine = this.SelectedLine,
                        SelectedMachine = this.SelectedMachine,
                        IPInfo = this.IPInfo,
                        PathToLineList = this.PathToLineList,
                        UseBasicContentHelper = HelperStatus.UseBasicContentHelper,
                        UseNetworkHelper = HelperStatus.UseNetworkHelper,
                        UseSpecificContentHelper = HelperStatus.UseSpecificContentHelper,
                    };

                    System.IO.File.WriteAllText(Define.BackupFilePath, JsonConvert.SerializeObject(backupData, Formatting.Indented));
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
                    NetworkAdapterPropertiesView napView = new NetworkAdapterPropertiesView(ref _IPInfo);
                    napView.ShowDialog();
                }));
            }
        }

        public ICommand ChangeLanguageCommand
        {
            get
            {
                return _ChangeLanguageCommand ?? (_ChangeLanguageCommand = new RelayCommand<object>((o) =>
                {
                    MenuItem menu = o as MenuItem;
                    App.SelectCulture(menu.Tag.ToString());

                    OnPropertyChanged(string.Empty);
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
                MessageBox.Show((string)Application.Current.FindResource("warningSelectLineAndMachine"));
                return false;
            }

            if (string.IsNullOrEmpty(PathToLineList))
            {
                MessageBox.Show((string)Application.Current.FindResource("warningSelectContentDirectoryFirst"));
                return false;
            }

            if (IPAddress.TryParse(IPInfo.IPString, out IPAddress ip) == false)
            {
                MessageBox.Show((string)Application.Current.FindResource("warningIpWrongFormat"));
                return false;
            }

            if (string.IsNullOrEmpty(InputMachineNumber))
            {
                MessageBox.Show((string)Application.Current.FindResource("warningIpMachineNumber"));
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
            IPInfo = backupData.IPInfo;
            PathToLineList = backupData.PathToLineList;

            HelperStatus.UseBasicContentHelper = backupData.UseBasicContentHelper;
            HelperStatus.UseNetworkHelper = backupData.UseNetworkHelper;
            HelperStatus.UseSpecificContentHelper = backupData.UseSpecificContentHelper;
        }

        private void PrintStartLog()
        {
            Define.Logger.ResetErrorCount();

            string startLog = 
                $"\n" +
                $"☛☛☛ START INSTALL MES\n" +
                $"☛☛☛ MACHINE : {SelectedLine}\\{SelectedMachine} #{InputMachineNumber}\n" +
                $"☛☛☛ IP      : {IPInfo.IPString} | {IPInfo.SubnetMask} | {IPInfo.DefaultGateway}\n";

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
        private IPData _IPInfo = new IPData();
        private string _PathToLineList;
        private string _InputMachineNumber;
        private int _CompletedPercentage;

        private ICommand _InstallStartCommand;
        private ICommand _LineListPathBrowseCommand;
        private ICommand _BackupDataCommand;
        private ICommand _NavigateCommand;
        private ICommand _ChangeLanguageCommand;
#endregion
    }
}
