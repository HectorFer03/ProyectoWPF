using FothelCards.MVVM.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FothelCards.MVVM.Model
{
    public class ConfiguracionModel : BaseViewModel
    {
        private string _resolucion = "1024x768";
        public string Resolucion { get => _resolucion; set { _resolucion = value; OnPropertyChanged(); } }

        private string _modoUso = "Modo Edición";
        public string ModoUso { get => _modoUso; set { _modoUso = value; OnPropertyChanged(); } }

        private double _volumen = 50;
        public double Volumen { get => _volumen; set { _volumen = value; OnPropertyChanged(); } }

        private bool _silenciarAudio = false; // Actuará como nuestro Toggle
        public bool SilenciarAudio { get => _silenciarAudio; set { _silenciarAudio = value; OnPropertyChanged(); } }
    }
}
