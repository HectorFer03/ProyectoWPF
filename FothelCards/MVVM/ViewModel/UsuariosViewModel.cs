using FothelCards.MVVM.Data;
using FothelCards.MVVM.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Security.Cryptography; // Importante para encriptar
using System.Text;
using System.Threading.Tasks;
using System.Windows; // Importante para la alerta de duplicados
using System.Windows.Input;

namespace FothelCards.MVVM.ViewModel
{
    public class UsuariosViewModel : BaseViewModel
    {
        public ObservableCollection<UsuarioModel> ListaUsuarios { get; set; }

        // Propiedades para el formulario de Alta
        private string _nuevoUsername;
        public string NuevoUsername { get => _nuevoUsername; set { _nuevoUsername = value; OnPropertyChanged(); } }

        private string _nuevoPassword;
        public string NuevoPassword { get => _nuevoPassword; set { _nuevoPassword = value; OnPropertyChanged(); } }

        private string _nuevoRol = "Empleado"; // Valor por defecto
        public string NuevoRol { get => _nuevoRol; set { _nuevoRol = value; OnPropertyChanged(); } }

        public ICommand EliminarUsuarioCommand { get; }
        public ICommand AgregarUsuarioCommand { get; } // Nuevo Comando

        public UsuariosViewModel()
        {
            ListaUsuarios = new ObservableCollection<UsuarioModel>();
            EliminarUsuarioCommand = new RelayCommand(EliminarUsuario);
            AgregarUsuarioCommand = new RelayCommand(AgregarUsuario); // Asignar el comando

            CargarUsuarios();
        }

        private async void CargarUsuarios()
        {
            ListaUsuarios.Clear();
            AccesoDatos db = new AccesoDatos();
            DataTable dt = await db.EjecutarProcedimientoAsync("sp_ObtenerUsuarios");

            foreach (DataRow fila in dt.Rows)
            {
                ListaUsuarios.Add(new UsuarioModel
                {
                    Id = (int)fila["id"],
                    Nombre = fila["username"].ToString(),
                    Rol = fila["rol"].ToString()
                });
            }
        }

        // 1. FUNCIÓN PARA ENCRIPTAR LA CONTRASEÑA CORRECTAMENTE
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

        private async void AgregarUsuario(object parameter)
        {
            // Validar que no estén vacíos
            if (string.IsNullOrWhiteSpace(NuevoUsername) || string.IsNullOrWhiteSpace(NuevoPassword)) return;

            // 2. VALIDACIÓN: EVITAR USUARIOS DUPLICADOS
            bool existeDuplicado = ListaUsuarios.Any(u => u.Nombre.Equals(NuevoUsername, StringComparison.OrdinalIgnoreCase));
            if (existeDuplicado)
            {
                MessageBox.Show($"El usuario '{NuevoUsername}' ya existe en el sistema. Por favor, elige otro nombre.",
                                "Usuario Duplicado",
                                MessageBoxButton.OK,
                                MessageBoxImage.Warning);
                return; // Detiene el proceso, no guarda en base de datos
            }

            // 3. ENCRIPTAMOS LA CONTRASEÑA ANTES DE MANDARLA A LA BD
            string passwordHash = HashPassword(NuevoPassword);

            AccesoDatos db = new AccesoDatos();
            await db.EjecutarProcedimientoNonQueryAsync(
                "sp_InsertarUsuario",
                new List<string> { "p_usuario", "p_password", "p_rol" },
                new List<object> { NuevoUsername, passwordHash, NuevoRol } // ¡Aquí se guarda el passwordHash!
            );

            // Limpiar formulario y recargar lista
            NuevoUsername = string.Empty;
            NuevoPassword = string.Empty;
            CargarUsuarios();
        }

        private async void EliminarUsuario(object parameter)
        {
            if (parameter is UsuarioModel usuario)
            {
                AccesoDatos db = new AccesoDatos();
                await db.EjecutarProcedimientoNonQueryAsync(
                    "sp_EliminarUsuario",
                    new List<string> { "p_id" },
                    new List<object> { usuario.Id }
                );
                ListaUsuarios.Remove(usuario);
            }
        }
    }
}