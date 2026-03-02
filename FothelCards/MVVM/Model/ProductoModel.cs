using FothelCards.MVVM.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FothelCards.MVVM.Model
{
    /// <summary>
    /// Modelo que representa un Producto (Carta) en la tienda.
    /// Hereda de BaseViewModel para actualizar la interfaz automáticamente cuando los valores cambian.
    /// </summary>
    public class ProductoModel : BaseViewModel
    {
        // Identificador único del producto en la Base de Datos
        public int Id { get; set; }

        // Nombre de la carta o producto.
        // Al modificar la propiedad, OnPropertyChanged() avisa a la UI (XAML) para que redibuje el nuevo texto.
        private string _nombre = string.Empty;
        public string Nombre { get => _nombre; set { _nombre = value; OnPropertyChanged(); } }

        // Tipo o Categoría del producto (Ej: 'Monstruo', 'Hechizo', etc.)
        private string _tipo = string.Empty;
        public string Tipo { get => _tipo; set { _tipo = value; OnPropertyChanged(); } }

        // Estado físico o disponibilidad del producto (Ej: 'Nuevo', 'Usado', 'Agotado')
        private string _estado = string.Empty;
        public string Estado { get => _estado; set { _estado = value; OnPropertyChanged(); } }

        // Ruta local o URL de la imagen de portada de la carta
        private string _rutaPortada = string.Empty;
        public string RutaPortada { get => _rutaPortada; set { _rutaPortada = value; OnPropertyChanged(); } }
    }
}
