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
    /// <summary>
    /// ViewModel para la administración de Empleados/Usuarios dentro de la tienda.
    /// Exclusivo usualmente para roles de Administrador.
    /// </summary>
    public class UsuariosViewModel : BaseViewModel
    {
        // Lista observable que volcará su contenido al DataGrid (Tabla Visual)
        public ObservableCollection<UsuarioModel> ListaUsuarios { get; set; }

        // Propiedades de almacenamiento temporal para el formulario de Alta (Añadir Trabajador)
        private string _nuevoUsername;
        public string NuevoUsername { get => _nuevoUsername; set { _nuevoUsername = value; OnPropertyChanged(); } }

        private string _nuevoPassword;
        public string NuevoPassword { get => _nuevoPassword; set { _nuevoPassword = value; OnPropertyChanged(); } }

        private string _nuevoRol = "Empleado"; // Valor por defecto del ComboBox de roles
        public string NuevoRol { get => _nuevoRol; set { _nuevoRol = value; OnPropertyChanged(); } }

        // Comandos / Botones
        public ICommand EliminarUsuarioCommand { get; }
        public ICommand AgregarUsuarioCommand { get; }

        public UsuariosViewModel()
        {
            ListaUsuarios = new ObservableCollection<UsuarioModel>();
            
            EliminarUsuarioCommand = new RelayCommand(EliminarUsuario);
            AgregarUsuarioCommand = new RelayCommand(AgregarUsuario);

            // Al cargar la vista, traemos la lista actual de empleados del MySQL
            CargarUsuarios();
        }

        /// <summary>
        /// (LEER) Trae de la base de datos a los usuarios y los expone en la tabla.
        /// </summary>
        private async void CargarUsuarios()
        {
            ListaUsuarios.Clear();
            AccesoDatos db = new AccesoDatos();
            DataTable dt = await db.EjecutarProcedimientoAsync("sp_ObtenerUsuarios");

            // Mapeo (Conversión Fila SQL -> Objeto C#)
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
        // Igual que en Login y Registro. Ninguna clave viaja en texto claro.
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
        /// (CREAR) Método administrativo para registrar empleados directamente desde el panel interno.
        /// </summary>
        private async void AgregarUsuario(object parameter)
        {
            // Validar que no estén vacíos
            if (string.IsNullOrWhiteSpace(NuevoUsername) || string.IsNullOrWhiteSpace(NuevoPassword)) return;

            // 2. VALIDACIÓN LÓGICA: EVITAR USUARIOS DUPLICADOS EN TIEMPO DE EJECUCIÓN
            // Busca en la memoria local si hay un apodo igual sin importar si son mayúsculas/minúsculas (StringComparison)
            bool existeDuplicado = ListaUsuarios.Any(u => u.Nombre.Equals(NuevoUsername, StringComparison.OrdinalIgnoreCase));
            if (existeDuplicado)
            {
                MessageBox.Show($"El usuario '{NuevoUsername}' ya existe en el sistema. Por favor, elige otro nombre.",
                                "Usuario Duplicado",
                                MessageBoxButton.OK,
                                MessageBoxImage.Warning);
                return; // Detiene el proceso, no guarda en base de datos cortándolo aquí
            }

            // 3. ENCRIPTAMOS LA CONTRASEÑA ANTES DE MANDARLA A LA BD
            string passwordHash = HashPassword(NuevoPassword);

            // Creamos conexión e insertamos en BD
            AccesoDatos db = new AccesoDatos();
            await db.EjecutarProcedimientoNonQueryAsync(
                "sp_InsertarUsuario",
                new List<string> { "p_usuario", "p_password", "p_rol" },
                new List<object> { NuevoUsername, passwordHash, NuevoRol } // ¡Aquí se guarda el passwordHash!
            );

            // Limpiar formulario manual tras añadir, y forzar la recarga de lista desde BD para traer la nueva ID.
            NuevoUsername = string.Empty;
            NuevoPassword = string.Empty;
            CargarUsuarios();
        }

        /// <summary>
        /// (ELIMINAR) Despide/Borra a un usuario. El parámetro viene por Binding del botón rojo de la cuadrícula.
        /// </summary>
        private async void EliminarUsuario(object parameter)
        {
            // Aseguramos que lo que envió el botón es un Objeto Usuario
            if (parameter is UsuarioModel usuario)
            {
                AccesoDatos db = new AccesoDatos();
                // Mandamos orden a MySQL
                await db.EjecutarProcedimientoNonQueryAsync(
                    "sp_EliminarUsuario",
                    new List<string> { "p_id" },
                    new List<object> { usuario.Id } // Usar primary Key para que sea un tiro certero
                );
                
                // Lo extraemos virtualmente de la vista para no hacer spam al servidor para recargar la lista
                ListaUsuarios.Remove(usuario);
            }
        }
    }
}