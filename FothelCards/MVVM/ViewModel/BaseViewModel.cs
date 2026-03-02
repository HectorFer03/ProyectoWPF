using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace FothelCards.MVVM.ViewModel
{
    /// <summary>
    /// ViewModel base del que heredarán todos los demás ViewModels (Login, Registro, Dashboard, etc.).
    /// Implementa INotifyPropertyChanged, que es el mecanismo estándar de WPF para "avisar" 
    /// a la Vista (XAML) de que una variable ha cambiado por detrás y la pantalla debe actualizarse.
    /// </summary>
    public class BaseViewModel : INotifyPropertyChanged
    {
        // Evento que se dispara cada vez que cambia el valor de una propiedad.
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Método de ayuda para disparar el evento PropertyChanged.
        /// [CallerMemberName] usa "magia" del compilador para saber automáticamente 
        /// el nombre de la variable que ha llamado a este método, ahorrándonos escribirlo a mano.
        /// </summary>
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            // El '?' comprueba que haya algo escuchando el evento antes de lanzarlo para evitar errores nulos
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
