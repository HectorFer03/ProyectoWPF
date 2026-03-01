using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Text;
using System.Windows.Controls;
using System.Windows.Input;
using System.Threading.Tasks;

namespace FothelCards.MVVM.ViewModel
{
    public class LoginViewModel : BaseViewModel
    {
        public string Username { get; set; }
        public string Password { get; set; }

        private string _mensaje;
        public string Mensaje
        {
            get => _mensaje;
            set { _mensaje = value; OnPropertyChanged(); }
        }

        public ICommand LoginCommand { get; }
        public ICommand BypassCommand { get; }

        public LoginViewModel()
        {
            LoginCommand = new RelayCommand(EjecutarLogin);
            BypassCommand = new RelayCommand(HacerBypass);
        }

        private void EjecutarLogin(object parameter)
        {
            // Login básico y normal
            if (Username == "admin" && Password == "1234")
            {
                NavegarAlDashboard();
            }
            else
            {
                Mensaje = "Usuario o contraseña incorrectos.";
            }
        }

        private void HacerBypass(object parameter)
        {
            // Botón de invitado rápido que pide el PDF
            NavegarAlDashboard();
        }

        private void NavegarAlDashboard()
        {
            // Forma sencilla de navegar usando el Frame del MainWindow
            var mainWindow = (MainWindow)Application.Current.MainWindow;
            mainWindow.MainFrame.Navigate(new FothelCards.MVVM.View.DashboardPage());
        }
    }
}
