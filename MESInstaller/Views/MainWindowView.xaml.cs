using MahApps.Metro.Controls;
using MESInstaller.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace MESInstaller.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindowView : MetroWindow
    {
        public MainWindowView()
        {
            InitializeComponent();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            var viewModel = this.DataContext as ViewModels.MainWindowViewModel;

            if (viewModel.BackupDataCommand.CanExecute(e))
            {
                viewModel.BackupDataCommand.Execute(e);
            }
        }
    }
}
