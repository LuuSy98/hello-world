using MESInstaller.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace MESInstaller.ViewModels
{
    public class NetworkAdapterPropertiesViewModel : PropertyChangedNotifier
    {
        #region Properties
        public IPData IPInfo
        {
            get { return _IPInfo; }
            set
            {
                _IPInfo = value;
                OnPropertyChanged("IPInfo");
            }
        }
        #endregion

        #region Commands
        public ICommand ConfirmCommand
        {
            get
            {
                return _ConfirmCommand ?? (_ConfirmCommand = new RelayCommand<Window>((window) =>
                {
                    if (window != null)
                    {
                        (window as Window).Close();
                    }
                }));
            }
        }
        #endregion

        #region Privates
        private IPData _IPInfo = new IPData();
        public ICommand _ConfirmCommand;
        #endregion
    }
}
