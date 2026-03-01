using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace FothelCards.MVVM.ViewModel
{
    public class DashboardViewModel : BaseViewModel
    {
        private object _vistaActual;
        public object VistaActual
        {
            get => _vistaActual;
            set { _vistaActual = value; OnPropertyChanged(); }
        }

        // Comandos de Navegación
        public ICommand MostrarStockCommand { get; }
        public ICommand MostrarUsuariosCommand { get; }
        public ICommand MostrarInfoCommand { get; }
        public ICommand SalirCommand { get; }
        public ICommand MostrarConfiguracionCommand { get; }

        public DashboardViewModel()
        {
            // Instanciamos las vistas (ViewModels)
            MostrarStockCommand = new RelayCommand(o => VistaActual = new WorkspaceViewModel());
            MostrarUsuariosCommand = new RelayCommand(o => VistaActual = new UsuariosViewModel());

            // Vista por defecto al iniciar el Dashboard
            VistaActual = new WorkspaceViewModel();

            MostrarInfoCommand = new RelayCommand(MostrarInfo);
            SalirCommand = new RelayCommand(CerrarSesion);
            MostrarConfiguracionCommand = new RelayCommand(o => VistaActual = new ConfiguracionViewModel());
        }

        private void MostrarInfo(object parameter)
        {
            MessageBox.Show("FothelCards v1.0\nDesarrollado para la evaluación de Interfaces.", "Acerca de", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void CerrarSesion(object parameter)
        {
            var mainWindow = (MainWindow)Application.Current.MainWindow;
            mainWindow.MainFrame.Navigate(new FothelCards.MVVM.View.LoginPage());
        }
    }
}
