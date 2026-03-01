using System;
using FothelCards.MVVM.Data;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Threading.Tasks;

namespace FothelCards.MVVM.ViewModel
{
    public class RegistroViewModel : BaseViewModel
    {
        public string Username { get; set; }
        public string Email { get; set; }

        private string _mensaje;
        public string Mensaje { get => _mensaje; set { _mensaje = value; OnPropertyChanged(); } }

        public ICommand RegistrarCommand { get; }
        public ICommand VolverCommand { get; }

        public RegistroViewModel()
        {
            RegistrarCommand = new RelayCommand(EjecutarRegistro);
            VolverCommand = new RelayCommand(NavegarAlLogin);
        }

        private async void EjecutarRegistro(object parameter)
        {
            var passwordBox = parameter as PasswordBox;
            string passwordReal = passwordBox?.Password ?? "";

            if (string.IsNullOrWhiteSpace(Username) || string.IsNullOrWhiteSpace(Email) || string.IsNullOrWhiteSpace(passwordReal))
            {
                Mensaje = "Rellena todos los campos.";
                return;
            }

            try
            {
                AccesoDatos db = new AccesoDatos();

                // 1. COMPROBAR DUPLICADOS (Cumple el criterio de Validación de Datos)
                DataTable dtChequeo = await db.EjecutarProcedimientoAsync(
                    "sp_VerificarDuplicado",
                    new List<string> { "p_usuario", "p_email" },
                    new List<object> { Username, Email }
                );

                if (dtChequeo.Rows.Count > 0)
                {
                    Mensaje = "El Usuario o Email ya están registrados.";
                    return;
                }

                // 2. HASHEAR Y GUARDAR
                string hash = HashPassword(passwordReal);
                await db.EjecutarProcedimientoNonQueryAsync(
                    "sp_InsertarUsuarioRegistro",
                    new List<string> { "p_usuario", "p_email", "p_password", "p_rol" },
                    new List<object> { Username, Email, hash, "Empleado" }
                );

                Mensaje = "Registro exitoso. Volviendo al login...";
                await System.Threading.Tasks.Task.Delay(1500); // Pequeña pausa
                NavegarAlLogin(null);
            }
            catch (Exception ex)
            {
                Mensaje = "Error en BD: " + ex.Message;
            }
        }

        private string HashPassword(string password)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                StringBuilder builder = new StringBuilder();
                foreach (byte b in bytes) builder.Append(b.ToString("x2"));
                return builder.ToString();
            }
        }

        private void NavegarAlLogin(object parameter)
        {
            var mainWindow = (MainWindow)Application.Current.MainWindow;
            mainWindow.MainFrame.Navigate(new FothelCards.MVVM.View.LoginPage());
        }
    }
}
