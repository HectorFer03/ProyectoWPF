using FothelCards.MVVM.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FothelCards.MVVM.Model
{
    /// <summary>
    /// Modelo que representa un Usuario del sistema.
    /// Hereda de BaseViewModel para poder notificar a la interfaz gráfica si alguna propiedad cambia (aunque en este modelo básico no se usen setters completos con OnPropertyChanged).
    /// </summary>
    public class UsuarioModel : BaseViewModel
    {
        // Identificador único del usuario (Primary Key en la BD)
        public int Id { get; set; }
        
        // Nombre del usuario (Ej: 'admin', 'hector')
        public string Nombre { get; set; } = string.Empty;
        
        // Rol del usuario dentro de la aplicación (Ej: 'Administrador', 'Cliente')
        public string Rol { get; set; } = string.Empty;
    }
}
