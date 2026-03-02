using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace FothelCards.MVVM.View
{
    /// <summary>
    /// Código detrás de 'AudioWidget'. Este es uno de los pocos casos donde está permitido tener lógica 
    /// en el Code-Behind dentro de MVVM, ya que es sólo lógica PURAMENTE VISUAL (mutear un reproductor),
    /// y no lógica de negocio (como guardar en base de datos).
    /// </summary>
    public partial class AudioWidget : UserControl
    {
        // Evento personalizado. El Dashboard estará "escuchando" a que este evento ocurra.
        public event RoutedEventHandler ToggleAudio;

        private bool _isMuted = false;

        public AudioWidget()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Se ejecuta cuando hacemos clic en el botón de Silenciar del propio Widget
        /// </summary>
        private void BtnMute_Click(object sender, RoutedEventArgs e)
        {
            // Cambia el estado (Si era false, ahora es true, y viceversa)
            _isMuted = !_isMuted;
            
            // Cambia el texto del botón basándose en el nuevo estado
            BtnMute.Content = _isMuted ? "Activar Audio" : "Silenciar Audio";

            // ¡Grita! "Ey Dashboard, han pulsado el botón".
            // Y entonces el Dashboard (Que tiene el MediaElement de verdad) pausa o reanuda la canción.
            ToggleAudio?.Invoke(this, new RoutedEventArgs());
        }
    }
}
