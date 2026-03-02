using FothelCards.MVVM.View;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace FothelCards.MVVM.ViewModel
{
    /// <summary>
    /// ViewModel que gestiona la estructura de pestañas principal (Menú Lateral).
    /// Permite la "navegación interna" cambiando de una vista a otra sin crear ventanas nuevas.
    /// </summary>
    public class DashboardViewModel : BaseViewModel
    {
        // Almacena qué Viewmodel está actualmente activo.
        // Al estar enlazado a la Vista, cambiar esto cambia todo el contenido del centro de la pantalla.
        private object _vistaActual;
        public object VistaActual
        {
            get => _vistaActual;
            set { _vistaActual = value; OnPropertyChanged(); }
        }

        // Comandos de Navegación del Menú Lateral
        public ICommand MostrarStockCommand { get; }
        public ICommand MostrarUsuariosCommand { get; }
        public ICommand MostrarInfoCommand { get; }
        public ICommand SalirCommand { get; }
        public ICommand MostrarConfiguracionCommand { get; }

        public DashboardViewModel()
        {
            // Instanciamos las acciones. Cuando pulsan un botón, asignamos a VistaActual un ViewModel diferente,
            // y WPF automáticamente lo dibuja gracias a los 'DataTemplates' definidos en 'App.xaml'.
            MostrarStockCommand = new RelayCommand(o => VistaActual = new WorkspaceViewModel());
            MostrarUsuariosCommand = new RelayCommand(o => VistaActual = new UsuariosViewModel());
            MostrarConfiguracionCommand = new RelayCommand(o => VistaActual = new ConfiguracionViewModel());
            
            MostrarInfoCommand = new RelayCommand(MostrarInfo);
            SalirCommand = new RelayCommand(CerrarSesion);

            // Vista por defecto al iniciar el Dashboard (La Tienda/Stock de Productos)
            VistaActual = new WorkspaceViewModel();
        }

        /// <summary>
        /// Muestra la página estática con la información legal y de desarrolladores del proyecto.
        /// </summary>
        private void MostrarInfo(object parameter)
        {
            // En vez de inyectar el ViewModel (que no hace falta porque es contenido estático), inyectamos la página entera XAML.
            VistaActual = new AcercaDePage();
        }

        /// <summary>
        /// Comando para destruir la sesión y devolver al usuario a la pantalla de entrada del programa.
        /// </summary>
        private void CerrarSesion(object parameter)
        {
            // Buscamos el contenedor padre (MainWindow) y volvemos sobre sus pasos a Login
            var mainWindow = (MainWindow)Application.Current.MainWindow;
            mainWindow.MainFrame.Navigate(new FothelCards.MVVM.View.LoginPage());
        }
    }
}
