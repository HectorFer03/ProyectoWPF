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
            ContenidoFrame.Navigate(new WorkspacePage());
        }

        private void BtnStock_Click(object sender, RoutedEventArgs e)
        {
            ContenidoFrame.Navigate(new WorkspacePage());
        }

        private void BtnUsuarios_Click(object sender, RoutedEventArgs e)
        {
            ContenidoFrame.Navigate(new UsuariosPage());
        }

        private void BtnSalir_Click(object sender, RoutedEventArgs e)
        {
            // Para cerrar sesión, accedemos al Frame principal del MainWindow y volvemos al Login
            var mainWindow = (MainWindow)Application.Current.MainWindow;
            mainWindow.MainFrame.Navigate(new LoginPage());
        }
    }
}
