using FothelCards.MVVM.Data;
using System;
using System.Security.Cryptography;
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
        public string Password { get; set; } // Nota: Idealmente se pasa desde la Vista al ejecutar el comando.

        private string _mensaje;
        public string Mensaje { get => _mensaje; set { _mensaje = value; OnPropertyChanged(); } }

        // Propiedad para bloquear la UI (Control de intentos)
        private bool _isUiEnabled = true;
        public bool IsUiEnabled { get => _isUiEnabled; set { _isUiEnabled = value; OnPropertyChanged(); } }

        private int _intentosFallidos = 0;

        public ICommand LoginCommand { get; }
        public ICommand BypassCommand { get; }
        public ICommand IrRegistroCommand { get; }
        public ICommand RecuperarPasswordCommand { get; }

        public LoginViewModel()
        {
            LoginCommand = new RelayCommand(EjecutarLogin);
            BypassCommand = new RelayCommand(HacerBypass);
            IrRegistroCommand = new RelayCommand(NavegarARegistro);
            RecuperarPasswordCommand = new RelayCommand(RecuperarPassword);
        }

        // Método de Criptografía Básica (SHA256)
        private string HashPassword(string password)
        {
            if (string.IsNullOrEmpty(password)) return string.Empty;
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                StringBuilder builder = new StringBuilder();
                foreach (byte b in bytes)
                    builder.Append(b.ToString("x2"));
                return builder.ToString();
            }
        }

        private async void EjecutarLogin(object parameter)
        {
            // Bloqueo de seguridad
            if (!IsUiEnabled) return;

            Mensaje = "Conectando...";
            try
            {
                // Hashear la contraseña antes de mandarla a la DB
                string passwordHash = HashPassword(Password);

                AccesoDatos db = new AccesoDatos();
                DataTable dt = await db.EjecutarProcedimientoAsync(
                    "sp_Login",
                    new List<string> { "p_usuario", "p_password" },
                    new List<object> { Username, passwordHash }
                );

                if (dt.Rows.Count == 1)
                {
                    _intentosFallidos = 0;
                    NavegarAlDashboard();
                }
                else
                {
                    _intentosFallidos++;
                    if (_intentosFallidos >= 3)
                    {
                        await BloquearUI();
                    }
                    else
                    {
                        Mensaje = $"Credenciales incorrectas. Intentos: {_intentosFallidos}/3";
                    }
                }
            }
            catch (Exception ex)
            {
                Mensaje = "Error de conexión: " + ex.Message;
            }
        }

        private async Task BloquearUI()
        {
            IsUiEnabled = false;
            Mensaje = "Demasiados intentos. Bloqueo de 30 segundos.";
            await Task.Delay(30000); // Espera asíncrona que no congela el hilo principal
            _intentosFallidos = 0;
            IsUiEnabled = true;
            Mensaje = "Puedes volver a intentarlo.";
        }

        // Simulación de envío SMTP para recuperación de contraseñas
        private void RecuperarPassword(object parameter)
        {
            if (string.IsNullOrWhiteSpace(Username))
            {
                Mensaje = "Introduce un usuario/email para recuperar la clave.";
                return;
            }
            // Aquí iría el código de System.Net.Mail.SmtpClient
            Mensaje = "Se ha enviado un correo de recuperación (Simulado).";
        }

        private void HacerBypass(object parameter) => NavegarAlDashboard();

        private void NavegarAlDashboard()
        {
            var mainWindow = (MainWindow)Application.Current.MainWindow;
            mainWindow.MainFrame.Navigate(new FothelCards.MVVM.View.DashboardPage());
        }

        private void NavegarARegistro(object parameter)
        {
            // var mainWindow = (MainWindow)Application.Current.MainWindow;
            // mainWindow.MainFrame.Navigate(new RegistroPage());
        }
    }
}

