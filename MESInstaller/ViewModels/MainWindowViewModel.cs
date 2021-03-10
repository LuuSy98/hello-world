using MESInstaller.Helpers;
using MESInstaller.Models;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Windows;
using System.Windows.Input;

namespace MESInstaller.ViewModels
{
    public class MainWindowViewModel : PropertyChangedNotifier
    {
        #region Properties
        public List<string> LineList
        {
            get { return _LineList; }
            set
            {
                _LineList = value;
                OnPropertyChanged("LineList");
            }
        }

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

        public string SelectedMachine
        {
            get { return _SelectedMachine; }
            set
            {
                _SelectedMachine = value;
                OnPropertyChanged("SelectedMachine");
            }
        }

        public string IPString
        {
            get { return _IPString; }
            set
            {
                _IPString = value;
                OnPropertyChanged("IPString");
            }
        }

        public string SubnetMask
        {
            get { return _SubnetMask; }
            set
            {
                _SubnetMask = value;
                OnPropertyChanged("SubnetMask");
            }
        }

        public string DefaultGateway
        {
            get { return _DefaultGateway; }
            set
            {
                _DefaultGateway = value;
                OnPropertyChanged("DefaultGateway");
            }
        }

        public string ContentPath
        {
            get { return _ContentPath; }
            set
            {
                _ContentPath = value;
                OnPropertyChanged("ContentPath");
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
                    if (string.IsNullOrEmpty(SelectedLine) || string.IsNullOrEmpty(SelectedMachine))
                    {
                        MessageBox.Show("Selecte line and machine first!");
                        return;
                    }

                    if (string.IsNullOrEmpty(ContentPath))
                    {
                        MessageBox.Show("Selecte content directory first!");
                        return;
                    }

                    IPAddress ip;
                    if (IPAddress.TryParse(IPString, out ip) == false)
                    {
                        MessageBox.Show("IP wrong format!");
                        return;
                    }

                    BasicContentHelper.ContentCopy(ContentPath);
                    NetworkHelper.SetIP(IPString, SubnetMask, DefaultGateway);
                }));
            }
        }

        public ICommand ContentBrowseCommand
        {
            get
            {
                return _ContentBrowseCommand ?? (_ContentBrowseCommand = new RelayCommand<object>((o) =>
                {
                    using (var dialog = new System.Windows.Forms.FolderBrowserDialog())
                    {
                        System.Windows.Forms.DialogResult result = dialog.ShowDialog();

                        if (result == System.Windows.Forms.DialogResult.OK)
                        {
                            ContentPath = dialog.SelectedPath;
                        }
                    }
                }));
            }
        }
        #endregion

        #region Constructor
        public MainWindowViewModel()
        {
            LineList = Define.LINE_LIST;
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
        private string _ContentPath;

        private ICommand _InstallStartCommand;
        private ICommand _ContentBrowseCommand;
        #endregion
    }
}
