using FothelCards.MVVM.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FothelCards.MVVM.Model
{
    /// <summary>
    /// Modelo que almacena las preferencias de configuración de la Aplicación.
    /// También usa OnPropertyChanged para que los controles visuales (como sliders o comboboxes) respondan al instante.
    /// </summary>
    public class ConfiguracionModel : BaseViewModel
    {
        // Resolución de la pantalla de la app (Ej: 1024x768).
        private string _resolucion = "1024x768";
        public string Resolucion { get => _resolucion; set { _resolucion = value; OnPropertyChanged(); } }

        // Modo de funcionamiento del programa (Ej: 'Modo Edición', 'Modo Lectura').
        private string _modoUso = "Modo Edición";
        public string ModoUso { get => _modoUso; set { _modoUso = value; OnPropertyChanged(); } }

        // Nivel de volumen general, con un valor por defecto de 50.
        private double _volumen = 50;
        public double Volumen { get => _volumen; set { _volumen = value; OnPropertyChanged(); } }

        // Interruptor (Toggle) para silenciar todo el audio del programa.
        // True = Silenciado, False = Con sonido.
        private bool _silenciarAudio = false;
        public bool SilenciarAudio { get => _silenciarAudio; set { _silenciarAudio = value; OnPropertyChanged(); } }
    }
}
