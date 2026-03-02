using Microsoft.Win32;
using FothelCards.MVVM.ViewModel;
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
    /// Lógica de interacción para WorkspacePage.xaml
    /// </summary>
    public partial class WorkspacePage : UserControl
    {
        public WorkspacePage()
        {
            InitializeComponent();
        }

        private void BtnBuscarImagen_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new OpenFileDialog();
            dialog.Filter = "Archivos de imagen|*.jpg;*.jpeg;*.png;*.bmp;*.gif";
            if (dialog.ShowDialog() == true)
            {
                var viewModel = (WorkspaceViewModel)this.DataContext;
                viewModel.NuevaRutaPortada = dialog.FileName;
            }
        }

        private void Grid_Drop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] archivos = (string[])e.Data.GetData(DataFormats.FileDrop);
                if (archivos.Length != 0)
                {
                    string rutaImagen = archivos[0];
                    string extension = System.IO.Path.GetExtension(rutaImagen).ToLower();

                    if (extension == ".jpg" || extension == ".jpeg" || extension == ".png" || extension == ".bmp" || extension == ".gif")
                    {
                        var viewModel = (WorkspaceViewModel)this.DataContext;
                        viewModel.NuevaRutaPortada = rutaImagen;
                    }
                }
            }
        }
    }
}
