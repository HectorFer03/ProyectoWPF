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
    public class ConfiguracionViewModel : BaseViewModel
    {
        private ConfiguracionModel _configuracion;
        public ConfiguracionModel Configuracion
        {
            get => _configuracion;
            set { _configuracion = value; OnPropertyChanged(); }
        }

        public ObservableCollection<string> OpcionesResolucion { get; set; }
        public ObservableCollection<string> OpcionesModo { get; set; }

        private string _mensaje;
        public string Mensaje { get => _mensaje; set { _mensaje = value; OnPropertyChanged(); } }

        public ICommand GuardarCommand { get; }
        public ICommand VolverCommand { get; }

        private readonly string _rutaConfig = "config.json";

        public ConfiguracionViewModel()
        {
            OpcionesResolucion = new ObservableCollection<string> { "800x600", "900x600", "1024x768", "1280x720" };
            OpcionesModo = new ObservableCollection<string> { "Modo Lectura", "Modo Edición", "Admin" };

            GuardarCommand = new RelayCommand(GuardarConfiguracion);
            VolverCommand = new RelayCommand(Volver);

            CargarConfiguracion();
        }

        private void CargarConfiguracion()
        {
            try
            {
                if (File.Exists(_rutaConfig))
                {
                    string json = File.ReadAllText(_rutaConfig);
                    Configuracion = JsonSerializer.Deserialize<ConfiguracionModel>(json) ?? new ConfiguracionModel();
                }
                else
                {
                    Configuracion = new ConfiguracionModel();
                }
                AplicarConfiguracion();
            }
            catch (Exception ex)
            {
                Mensaje = "Error al cargar configuración: " + ex.Message;
                Configuracion = new ConfiguracionModel();
            }
        }

        private void GuardarConfiguracion(object parameter)
        {
            try
            {
                var options = new JsonSerializerOptions { WriteIndented = true };
                string json = JsonSerializer.Serialize(Configuracion, options);
                File.WriteAllText(_rutaConfig, json);

                Mensaje = "Configuración guardada en config.json";
                AplicarConfiguracion();
            }
            catch (Exception ex)
            {
                Mensaje = "Error al guardar: " + ex.Message;
            }
        }

        private void AplicarConfiguracion()
        {
            // Aplica la resolución directamente a la ventana contenedora
            if (Application.Current.MainWindow != null)
            {
                var dimensiones = Configuracion.Resolucion.Split('x');
                if (dimensiones.Length == 2 &&
                    double.TryParse(dimensiones[0], out double width) &&
                    double.TryParse(dimensiones[1], out double height))
                {
                    Application.Current.MainWindow.Width = width;
                    Application.Current.MainWindow.Height = height;
                }
            }
        }

        private void Volver(object parameter)
        {
            Mensaje = "Usa el menú lateral para navegar a otro panel.";
        }
    }
}
