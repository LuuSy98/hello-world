using MESInstaller.Helpers;
using MESInstaller.Models;
using System;
using System.Collections.Generic;
using System.Text;
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
        #endregion

        #region Command
        private ICommand _InstallStartCommand;
        public ICommand InstallStartCommand
        {
            get
            {
                return _InstallStartCommand ?? (_InstallStartCommand = new RelayCommand<object>((o) =>
                {
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
        #endregion
    }
}
