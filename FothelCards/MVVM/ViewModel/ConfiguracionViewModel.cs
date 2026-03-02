using FothelCards.MVVM.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using System.IO;
using System.Text.Json;
using System.Windows;
using System.Windows.Input;
using System.Threading.Tasks;

namespace FothelCards.MVVM.ViewModel
{
    /// <summary>
    /// ViewModel encargado de gestionar los Ajustes / Preferencias Locales del programa.
    /// A diferencia de usuarios y cartas que van a MySQL, esta configuración se guarda en un archivo local .json
    /// </summary>
    public class ConfiguracionViewModel : BaseViewModel
    {
        // Instancia del Modelo de Configuración donde se guardan las variables (Volumen, Modo, etc)
        private ConfiguracionModel _configuracion;
        public ConfiguracionModel Configuracion
        {
            get => _configuracion;
            set { _configuracion = value; OnPropertyChanged(); }
        }

        // Listas desplegables para alimentar los ComboBox del diseño
        public ObservableCollection<string> OpcionesResolucion { get; set; }
        public ObservableCollection<string> OpcionesModo { get; set; }

        // Mensaje de feedback en pantalla para saber si se guardó bien
        private string _mensaje;
        public string Mensaje { get => _mensaje; set { _mensaje = value; OnPropertyChanged(); } }

        public ICommand GuardarCommand { get; }
        public ICommand VolverCommand { get; }

        // Ruta del archivo físico en el disco duro (se guardará en la misma carpeta del ejecutable /bin)
        private readonly string _rutaConfig = "config.json";

        public ConfiguracionViewModel()
        {
            // Opciones disponibles para la UI
            OpcionesResolucion = new ObservableCollection<string> { "800x600", "900x600", "1024x768", "1280x720" };
            OpcionesModo = new ObservableCollection<string> { "Modo Lectura", "Modo Edición", "Admin" };

            GuardarCommand = new RelayCommand(GuardarConfiguracion);
            VolverCommand = new RelayCommand(Volver);

            // Al abrir la pantalla de Ajustes, intentamos leer el fichero .json
            CargarConfiguracion();
        }

        /// <summary>
        /// (LEER) Extrae el texto del config.json y lo transforma a un objeto 'ConfiguracionModel' de C#
        /// </summary>
        private void CargarConfiguracion()
        {
            try
            {
                // Si el archivo ya existe (no es la primera vez que se abre la app)
                if (File.Exists(_rutaConfig))
                {
                    string json = File.ReadAllText(_rutaConfig);
                    // Deserializamos: Magia que convierte un Texto JSON a un Objeto real
                    Configuracion = JsonSerializer.Deserialize<ConfiguracionModel>(json) ?? new ConfiguracionModel();
                }
                else
                {
                    // Si nunca abrieron el programa, creamos una configuración base por defecto nueva
                    Configuracion = new ConfiguracionModel();
                }
                // Tras cargar los datos, hacemos que surtan efecto
                AplicarConfiguracion();
            }
            catch (Exception ex)
            {
                Mensaje = "Error al cargar configuración: " + ex.Message;
                Configuracion = new ConfiguracionModel();
            }
        }

        /// <summary>
        /// (ESCRIBIR) Agarra el objeto 'ConfiguracionModel' con los cambios del usuario y lo aplasta en el config.json
        /// </summary>
        private void GuardarConfiguracion(object parameter)
        {
            try
            {
                // Serializamos (Convertimos Objeto en texto JSON). 'WriteIndented' hace que se guarde bonito y tabulado
                var options = new JsonSerializerOptions { WriteIndented = true };
                string json = JsonSerializer.Serialize(Configuracion, options);
                File.WriteAllText(_rutaConfig, json);

                Mensaje = "Configuración guardada en config.json";
                AplicarConfiguracion(); // Aplicar cambios al instante
            }
            catch (Exception ex)
            {
                Mensaje = "Error al guardar: " + ex.Message;
            }
        }

        /// <summary>
        /// Coge los valores de la configuración y fuerza a la Ventana de WPF a cambiar de tamaño o hacer acciones en consecuencia.
        /// </summary>
        private void AplicarConfiguracion()
        {
            // Aplica la resolución directamente a la ventana contenedora principal
            if (Application.Current.MainWindow != null)
            {
                // Separa "1024x768" en "1024" y "768"
                var dimensiones = Configuracion.Resolucion.Split('x');
                
                // Si se pudo trocear bien y son números válidos...
                if (dimensiones.Length == 2 &&
                    double.TryParse(dimensiones[0], out double width) &&
                    double.TryParse(dimensiones[1], out double height))
                {
                    // Forzamos el Ancho y Alto de la ventana principal
                    Application.Current.MainWindow.Width = width;
                    Application.Current.MainWindow.Height = height;
                }
            }
        }

        /// <summary>
        /// Mensaje indicativo para volver, ya que la navegación global la maneja ahora el DashboardViewModel
        /// a través de la barra lateral izquierda en vez de botones internos por página.
        /// </summary>
        private void Volver(object parameter)
        {
            Mensaje = "Usa el menú lateral para navegar a otro panel.";
        }
    }
}
