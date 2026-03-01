using FothelCards.MVVM.Data;
using System;
using System.Security.Cryptography;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Net.Mail;
using System.Net;

namespace FothelCards.MVVM.ViewModel
{
    public class LoginViewModel : BaseViewModel
    {
        public string Username { get; set; }
        // Eliminamos public string Password porque ahora lo sacamos del PasswordBox

        private string _mensaje;
        public string Mensaje { get => _mensaje; set { _mensaje = value; OnPropertyChanged(); } }

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
            if (!IsUiEnabled) return;

            // Sacamos la contraseña del PasswordBox
            var passwordBox = parameter as PasswordBox;
            string passwordReal = passwordBox?.Password ?? "";

            Mensaje = "Conectando...";
            try
            {
                string passwordHash = HashPassword(passwordReal);

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
            await Task.Delay(30000);
            _intentosFallidos = 0;
            IsUiEnabled = true;
            Mensaje = "Puedes volver a intentarlo.";
        }

        private void RecuperarPassword(object parameter)
        {
            if (string.IsNullOrWhiteSpace(Username))
            {
                Mensaje = "Introduce tu email en el campo 'Usuario / Email' para recuperar.";
                return;
            }

            try
            {
                Mensaje = "Enviando correo...";
                var smtpClient = new SmtpClient("smtp.gmail.com")
                {
                    Port = 587,
                    Credentials = new NetworkCredential("tu_correo_proyecto@gmail.com", "tu_contraseña_de_aplicacion"),
                    EnableSsl = true,
                };

                var mailMessage = new MailMessage
                {
                    From = new MailAddress("tu_correo_proyecto@gmail.com"),
                    Subject = "Recuperación de Contraseña - FothelCards",
                    Body = "Has solicitado recuperar tu contraseña. Por favor contacta con el administrador.",
                    IsBodyHtml = true,
                };
                mailMessage.To.Add(Username);

                smtpClient.Send(mailMessage);
                Mensaje = "Correo enviado con éxito. Revisa tu bandeja de entrada.";
            }
            catch (Exception)
            {
                Mensaje = "Error al enviar correo. Verifica conexión y SMTP.";
            }
        }

        private void HacerBypass(object parameter) => NavegarAlDashboard();

        private void NavegarAlDashboard()
        {
            var mainWindow = (MainWindow)Application.Current.MainWindow;
            mainWindow.MainFrame.Navigate(new FothelCards.MVVM.View.DashboardPage());
        }

        private void NavegarARegistro(object parameter)
        {
            var mainWindow = (MainWindow)Application.Current.MainWindow;
            mainWindow.MainFrame.Navigate(new FothelCards.MVVM.View.RegistroPage());
        }
    }
}
