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
    /// <summary>
    /// ViewModel que controla la pantalla de Creación de nuevo usuario (RegistroPage.xaml).
    /// </summary>
    public class RegistroViewModel : BaseViewModel
    {
        // Propiedades enlazadas a los campos de texto del formulario
        public string Username { get; set; }
        public string Email { get; set; }

        // Mensaje de estado ("Registro exitoso", "El usuario ya existe") devuelto a la UI
        private string _mensaje;
        public string Mensaje { get => _mensaje; set { _mensaje = value; OnPropertyChanged(); } }

        // Comandos de los botones
        public ICommand RegistrarCommand { get; }
        public ICommand VolverCommand { get; }

        public RegistroViewModel()
        {
            RegistrarCommand = new RelayCommand(EjecutarRegistro);
            VolverCommand = new RelayCommand(NavegarAlLogin);
        }

        /// <summary>
        /// Flujo principal que se dispara al pulsar el botón de crear cuenta.
        /// </summary>
        private async void EjecutarRegistro(object parameter)
        {
            // Extraer de forma segura el texto de la caja de contraseñas de WPF
            var passwordBox = parameter as PasswordBox;
            string passwordReal = passwordBox?.Password ?? "";

            // Validación básica en local: Evitar que manden el formulario vacío
            if (string.IsNullOrWhiteSpace(Username) || string.IsNullOrWhiteSpace(Email) || string.IsNullOrWhiteSpace(passwordReal))
            {
                Mensaje = "Rellena todos los campos.";
                return;
            }

            try
            {
                AccesoDatos db = new AccesoDatos();

                // 1. COMPROBAR DUPLICADOS (Cumple el criterio de Validación de Datos)
                // Llama a la BD para ver si alguien ya ha robado ese apodo o email
                DataTable dtChequeo = await db.EjecutarProcedimientoAsync(
                    "sp_VerificarDuplicado",
                    new List<string> { "p_usuario", "p_email" },
                    new List<object> { Username, Email }
                );

                // Si la tabla devuelta tiene al menos una fila, ¡Ese usuario ya existe! Abortar misión.
                if (dtChequeo.Rows.Count > 0)
                {
                    Mensaje = "El Usuario o Email ya están registrados.";
                    return;
                }

                // 2. HASHEAR Y GUARDAR
                // Jamás guardamos contraseñas "1234". Las pasamos por un algoritmo irreversible primero.
                string hash = HashPassword(passwordReal);
                
                // Ejecutamos insertar. Observa que fuerzamos el rol "Empleado" por defecto.
                await db.EjecutarProcedimientoNonQueryAsync(
                    "sp_InsertarUsuarioRegistro",
                    new List<string> { "p_usuario", "p_email", "p_password", "p_rol" },
                    new List<object> { Username, Email, hash, "Empleado" }
                );

                Mensaje = "Registro exitoso. Volviendo al login...";
                
                // Pausamos artificialmente 1.5 segundos para que el usuario pueda leer el mensaje de éxito
                await System.Threading.Tasks.Task.Delay(1500); 
                
                // Redirigimos al usuario otra vez a la pantalla de Login
                NavegarAlLogin(null);
            }
            catch (Exception ex)
            {
                Mensaje = "Error en BD: " + ex.Message;
            }
        }

        /// <summary>
        /// Mismo algoritmo de encriptado SHA-256 usado en el Login para las claves.
        /// </summary>
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

        /// <summary>
        /// Cancela el proceso y vuelve a la pantalla de Iniciar Sesión.
        /// </summary>
        private void NavegarAlLogin(object parameter)
        {
            var mainWindow = (MainWindow)Application.Current.MainWindow;
            mainWindow.MainFrame.Navigate(new FothelCards.MVVM.View.LoginPage());
        }
    }
}
