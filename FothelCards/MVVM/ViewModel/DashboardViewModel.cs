using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FothelCards.MVVM.Model;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using System.Threading.Tasks;

namespace FothelCards.MVVM.ViewModel
{
    public class DashboardViewModel : BaseViewModel
    {
        public ObservableCollection<ProductoModel> ListaProductos { get; set; }

        // Parámetros para la grilla que pide el PDF
        public int Filas { get; set; } = 3;
        public int Columnas { get; set; } = 4;

        public ICommand GenerarCommand { get; }
        public ICommand CambiarEstadoCommand { get; }
        public ICommand SalirCommand { get; }

        public DashboardViewModel()
        {
            ListaProductos = new ObservableCollection<ProductoModel>();

            GenerarCommand = new RelayCommand(GenerarTienda);
            CambiarEstadoCommand = new RelayCommand(CambiarEstadoProducto);
            SalirCommand = new RelayCommand(Salir);
        }

        private void GenerarTienda(object parameter)
        {
            ListaProductos.Clear();
            int total = Filas * Columnas;
            for (int i = 0; i < total; i++)
            {
                ListaProductos.Add(new ProductoModel { Nombre = $"Caja {i + 1}", Estado = "Disponible" });
            }
        }

        private void CambiarEstadoProducto(object parameter)
        {
            if (parameter is ProductoModel producto)
            {
                // Alterna entre Disponible y Agotado al hacer clic
                producto.Estado = producto.Estado == "Disponible" ? "Agotado" : "Disponible";
            }
        }

        private void Salir(object parameter)
        {
            var mainWindow = (MainWindow)Application.Current.MainWindow;
            mainWindow.MainFrame.Navigate(new FothelCards.MVVM.View.LoginPage());
        }
    }
}
