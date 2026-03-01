using FothelCards.MVVM.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FothelCards.MVVM.Model
{
    public class ProductoModel : BaseViewModel
    {
        public int Id { get; set; }

        private string _nombre = string.Empty;
        public string Nombre { get => _nombre; set { _nombre = value; OnPropertyChanged(); } }

        private string _tipo = string.Empty;
        public string Tipo { get => _tipo; set { _tipo = value; OnPropertyChanged(); } }

        private string _estado = string.Empty;
        public string Estado { get => _estado; set { _estado = value; OnPropertyChanged(); } }

        private string _rutaPortada = string.Empty;
        public string RutaPortada { get => _rutaPortada; set { _rutaPortada = value; OnPropertyChanged(); } }
    }
}
