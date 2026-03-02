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
    /// Código detrás (Code-Behind) de la pantalla de Login visual (LoginPage.xaml).
    /// Siguiendo la arquitectura limpia MVVM elegida para el proyecto, todo el "code-behind" 
    /// como este debe mantenerse vacío de lógica de negocio o conexiones con la base de datos.
    /// Únicamente debe servir para inicializar sus dependencias visuales inherentes mediante el constructor.
    /// </summary>
    public partial class LoginPage : Page
    {
        public LoginPage()
        {
            // Inicializa los componentes declarados visualmente en el XAML y realiza los Bindings pertinentes.
            InitializeComponent();
        }
    }
}
