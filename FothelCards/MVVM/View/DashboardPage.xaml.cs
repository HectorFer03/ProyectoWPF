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
    /// Lógica de interacción para DashboardPage.xaml
    /// </summary>
    public partial class DashboardPage : Page
    {
        public DashboardPage()
        {
            InitializeComponent();
            
        }

        private void AudioWidget_ToggleAudio(object sender, System.Windows.RoutedEventArgs e)
        {
            if (BackgroundAudio.IsMuted)
            {
                BackgroundAudio.IsMuted = false;
            }
            else
            {
                BackgroundAudio.IsMuted = true;
            }
        }
    }
}
