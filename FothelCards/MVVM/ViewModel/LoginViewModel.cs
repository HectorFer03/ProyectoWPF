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
    /// <summary>
    /// ViewModel que controla toda la lógica detrás de la pantalla de inicio de sesión (LoginPage.xaml).
    /// Se encarga de validar credenciales, manejar bloqueos por intentos fallidos y recuperar contraseñas.
    /// </summary>
    public class LoginViewModel : BaseViewModel
    {
        // Propiedad enlazada al cuadro de texto del usuario en el diseño
        public string Username { get; set; }
        // Eliminamos public string Password porque ahora lo sacamos del PasswordBox de forma segura

        // Mensaje de estado ("Conectando...", "Credenciales incorrectas") que se muestra en pantalla
        private string _mensaje;
        public string Mensaje { get => _mensaje; set { _mensaje = value; OnPropertyChanged(); } }

        // Controla si los botones y campos de texto se pueden usar. Se pone a false al bloquear la UI.
        private bool _isUiEnabled = true;
        public bool IsUiEnabled { get => _isUiEnabled; set { _isUiEnabled = value; OnPropertyChanged(); } }

        // Contador interno para bloquear tras 3 fallos
        private int _intentosFallidos = 0;

        // Comandos (Acciones de los botones de la interfaz)
        public ICommand LoginCommand { get; }
        public ICommand BypassCommand { get; }
        public ICommand IrRegistroCommand { get; }
        public ICommand RecuperarPasswordCommand { get; }

        public LoginViewModel()
        {
            // Inicializamos los comandos asignándoles el método interno que deben ejecutar al ser pulsados
            LoginCommand = new RelayCommand(EjecutarLogin);
            BypassCommand = new RelayCommand(HacerBypass);
            IrRegistroCommand = new RelayCommand(NavegarARegistro);
            RecuperarPasswordCommand = new RelayCommand(RecuperarPassword);
        }

        /// <summary>
        /// Recibe una contraseña en texto plano y la convierte en un código ilegible de 256 bits (Hash SHA-256).
        /// Esto se hace para comparar con la base de datos sin enviar nunca la contraseña original.
        /// </summary>
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

        /// <summary>
        /// Lógica de autenticación principal al pulsar "Iniciar Sesión".
        /// </summary>
        private async void EjecutarLogin(object parameter)
        {
            // Si la interfaz está bloqueada (penalty por 3 fallos), no hacemos nada
            if (!IsUiEnabled) return;

            // Al WPF no le gusta enlazar (Binding) contraseñas por seguridad, así que extraemos el objeto PasswordBox que nos pasaron como parámetro desde el XAML
            var passwordBox = parameter as PasswordBox;
            string passwordReal = passwordBox?.Password ?? "";

            Mensaje = "Conectando...";
            try
            {
                // Hasheamos la contraseña tecleada para ver si coincide con el hash guardado en MySQL
                string passwordHash = HashPassword(passwordReal);

                // Llamamos a la base de datos usando nuestro AccesoDatos
                AccesoDatos db = new AccesoDatos();
                DataTable dt = await db.EjecutarProcedimientoAsync(
                    "sp_Login", // Procedimiento almacenado creado en la base de datos
                    new List<string> { "p_usuario", "p_password" }, // Nombres de los parámetros en el PA
                    new List<object> { Username, passwordHash } // Valores introducidos por el usuario
                );

                // Si la consulta devuelve exactamente 1 fila, es que las credenciales son correctas
                if (dt.Rows.Count == 1)
                {
                    _intentosFallidos = 0; // Reseteamos el contador de fallos
                    NavegarAlDashboard(); // Entramos al sistema
                }
                else
                {
                    // Si no devuelve filas, credenciales incorrectas. Sumamos 1 fallo.
                    _intentosFallidos++;
                    if (_intentosFallidos >= 3)
                    {
                        // A los 3 fallos, llamamos a la penalización
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
                // Si la base de datos está caída o no hay internet
                Mensaje = "Error de conexión: " + ex.Message;
            }
        }

        /// <summary>
        /// Con 3 intentos fallidos, deshabilita visualmente los botones durante 30 segundos.
        /// </summary>
        private async Task BloquearUI()
        {
            IsUiEnabled = false; // El XAML desactiva la UI al detectar esto (Binding)
            Mensaje = "Demasiados intentos. Bloqueo de 30 segundos.";
            
            // Pausamos la ejecución *solo en este punto* 30 segs, sin bloquear o colgar completamente la aplicación entera (Task.Delay asíncrono)
            await Task.Delay(30000);
            
            _intentosFallidos = 0;
            IsUiEnabled = true;
            Mensaje = "Puedes volver a intentarlo.";
        }

        /// <summary>
        /// Método que se ejecuta al pulsar el botón "¿Estás sordo/Olvidaste clave?".
        /// Intenta enviar un correo de aviso usando SMTP (Actualizar con credenciales reales).
        /// </summary>
        private void RecuperarPassword(object parameter)
        {
            // Verificamos si escribió un email válido o al menos rellenó la caja de texto
            if (string.IsNullOrWhiteSpace(Username))
            {
                Mensaje = "Introduce tu email en el campo 'Usuario / Email' para recuperar.";
                return;
            }

            try
            {
                Mensaje = "Enviando correo...";
                // Configuración estándar del servidor de correos de Gmail
                var smtpClient = new SmtpClient("smtp.gmail.com")
                {
                    Port = 587,
                    Credentials = new NetworkCredential("tu_correo_proyecto@gmail.com", "tu_contraseña_de_aplicacion"),
                    EnableSsl = true,
                };

                // Contenido del correo electrónico a enviar
                var mailMessage = new MailMessage
                {
                    // Quien lo envía
                    From = new MailAddress("tu_correo_proyecto@gmail.com"),
                    Subject = "Recuperación de Contraseña - FothelCards",
                    Body = "Has solicitado recuperar tu contraseña. Por favor contacta con el administrador.",
                    IsBodyHtml = true, // Permite usar código HTML <b> <i> para darle estilo al cuerpo del correo
                };
                
                // Le pasamos el Destinatario (Que es el usuario escrito en la caja de login)
                mailMessage.To.Add(Username);

                smtpClient.Send(mailMessage);
                Mensaje = "Correo enviado con éxito. Revisa tu bandeja de entrada.";
            }
            catch (Exception)
            {
                Mensaje = "Error al enviar correo. Verifica conexión y SMTP.";
            }
        }

        // Botón de emergencia para desarrollo. Entra directo sin comprobar en la Base de Datos.
        private void HacerBypass(object parameter) => NavegarAlDashboard();

        /// <summary>
        /// Busca la Ventana Principal del programa y le dice que reemplace su contenido actual por la página del Dashboard.
        /// Básicamente funciona como cambiar de pestaña o de escena.
        /// </summary>
        private void NavegarAlDashboard()
        {
            // Application.Current.MainWindow nos da acceso global a la ventana en ejecución
            var mainWindow = (MainWindow)Application.Current.MainWindow;
            mainWindow.MainFrame.Navigate(new FothelCards.MVVM.View.DashboardPage());
        }

        /// <summary>
        /// Cambia a la pantalla de Registro de nuevo Usuario.
        /// </summary>
        private void NavegarARegistro(object parameter)
        {
            var mainWindow = (MainWindow)Application.Current.MainWindow;
            mainWindow.MainFrame.Navigate(new FothelCards.MVVM.View.RegistroPage());
        }
    }
}
