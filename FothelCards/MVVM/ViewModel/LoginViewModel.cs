using FothelCards.MVVM.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

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

        private async void EjecutarLogin(object parameter)
        {
            Mensaje = "Conectando a la base de datos...";
            try
            {
                AccesoDatos db = new AccesoDatos();
                DataTable dt = await db.EjecutarProcedimientoAsync(
                    "sp_Login",
                    new List<string> { "p_usuario", "p_password" },
                    new List<object> { Username, Password }
                );

                if (dt.Rows.Count == 1)
                {
                    NavegarAlDashboard();
                }
                else
                {
                    Mensaje = "Usuario o contraseña incorrectos.";
                }
            }
            catch (Exception ex)
            {
                Mensaje = "Error de conexión: " + ex.Message;
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
