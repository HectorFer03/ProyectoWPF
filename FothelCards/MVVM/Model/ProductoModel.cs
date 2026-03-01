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
        public string Nombre { get; set; } = string.Empty;

        private string _estado;
        public string Estado
        {
            get => _estado;
            set { _estado = value; OnPropertyChanged(); }
        }
    }
}
